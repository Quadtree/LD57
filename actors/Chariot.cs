using System;
using System.Linq;
using Godot;

public partial class Chariot : Node3D
{
    [Export]
    float BuoyancyChangeRate;

    [Export]
    float MaxBuoyancy;

    [Export]
    float MinBuoyancy;

    [Export]
    float LeftRightRatio = 3;

    [Export]
    public float Health = 100;

    [Export]
    public float SlapRange = 8;

    [Export]
    public float GrabRange = 8;

    RigidBody3D CurrentlyGrabbed;

    Vector3 PrevVelocity;
    float PrevRotation;

    float RecentCollision = 0;

    [Export]
    public float SlapCooldown = 2;

    float SlapCooldownLeft = 0;

    [Export]
    public float InvulnerableTime = 2;

    [Export]
    public float StartingMainBuoyancy = 100;

    [Export]
    public float HorsePower = 300;

    [Export]
    public bool LightEnabled = false;

    float OriginalHorseMass = 0;
    float OriginalTotalNonHorseMass = 0;

    public static int KelpCollectedTotal = 0;
    public static int SuperGemsCollectedTotal = 0;

    public int KelpCollectedThisLevel = 0;
    public int SuperGemsCollectedThisLevel = 0;

    Node3D TentacleTarget;

    float TentacleTargetTime = 0;

    public override void _Ready()
    {
        base._Ready();

        if (LightEnabled)
        {
            foreach (var it in this.FindChildrenByType<OmniLight3D>()) it.Visible = true;
        }
        else
        {
            var minst = this.FindChildByName<MeshInstance3D>("Cube_003");
            if (minst.GetActiveMaterial(0) is StandardMaterial3D mat)
            {
                mat.EmissionEnabled = false;
            }
        }

        this.FindChildByName<Node>("DriverHead").FindChildByType<Buoyancy>().Amount = Util.Clamp(StartingMainBuoyancy, MinBuoyancy, MaxBuoyancy);

        OriginalHorseMass = this.FindChildByName<RigidBody3D>("SeaHorse").Mass;
        OriginalTotalNonHorseMass = this.FindChildrenByType<RigidBody3D>().Where(it => it.Name != "SeaHorse").Select(it => it.Mass).Sum();

        var myParts = this.FindChildrenByType<RigidBody3D>().ToArray();

        foreach (var it in myParts)
        {
            if (it.Name == "SeaHorse") continue;

            it.ContactMonitor = true;
            it.MaxContactsReported = 20;

            it.BodyEntered += (body) =>
            {
                if (!myParts.Contains(body) && !(body is AggressiveFish) && !(body is Grabbable))
                {
                    RecentCollision = 0.1f;

                    // var otherSpeed = new Vector3(0, 0, 0);
                    // if (body is RigidBody3D rb3d)
                    // {
                    //     otherSpeed = rb3d.LinearVelocity;
                    // }
                    // var relSpeed = otherSpeed.DistanceTo(it.LinearVelocity);

                    // var damage = Util.Clamp((relSpeed - 0.1f) * (5f / .2f), 0, 1000);

                    // GD.Print($"{it} has hit {body}, relSpeed={relSpeed} damage={damage}");

                    // Health -= damage;
                }
            };
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        var debugInfo = "";

        var moveVector = new Vector2(0, 0);

        var mbb = this.FindChildByName<Buoyancy>("MainBodyBuoyancy");
        var oldB = mbb.Amount;

        if (Input.IsActionPressed("move_left")) moveVector.X = -1;
        if (Input.IsActionPressed("move_right")) moveVector.X = 1;
        if (Input.IsActionPressed("move_up"))
        {
            moveVector.Y = 1 / LeftRightRatio;
            mbb.Amount = Util.Clamp(mbb.Amount + ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        }
        if (Input.IsActionPressed("move_down"))
        {
            moveVector.Y = -1 / LeftRightRatio;
            mbb.Amount = Util.Clamp(mbb.Amount - ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        }

        // var horseMass = this.FindChildByName<RigidBody3D>("SeaHorse").Mass;
        // var totalNonHorseMass = this.FindChildrenByType<RigidBody3D>().Where(it => it.Name != "SeaHorse").Select(it => it.Mass).Sum();
        // var horsePowerModifier = (totalNonHorseMass / OriginalHorseMass) / (OriginalTotalNonHorseMass / OriginalHorseMass);

        // debugInfo += $"OriginalHorseMass={OriginalHorseMass} OriginalTotalNonHorseMass={OriginalTotalNonHorseMass}\n";
        // debugInfo += $"horseMass={horseMass} totalNonHorseMass={totalNonHorseMass} \nhorsePowerModifier={horsePowerModifier}\n";

        var horsePowerModifier = 1;

        this.FindChildByName<RigidBody3D>("SeaHorse").Mass = OriginalHorseMass * horsePowerModifier;
        this.FindChildByName<RigidBody3D>("SeaHorse").ApplyCentralForce(new Vector3(moveVector.X, moveVector.Y, 0) * HorsePower * horsePowerModifier);

        if (Input.IsActionPressed("increase_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount + ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (Input.IsActionPressed("decrease_buoyancy")) mbb.Amount = Util.Clamp(mbb.Amount - ((float)delta * BuoyancyChangeRate), MinBuoyancy, MaxBuoyancy);
        if (mbb.Amount != oldB)
        {
            //GD.Print($"mbb.Amount={mbb.Amount}");
        }

        this.FindChildByName<Node3D>("DriverHeadMesh").Scale = Vector3.One * (Mathf.Pow(mbb.Amount, .333333f) / 5);

        // move camera to follow head
        var cam = this.FindChildByName<Node3D>("ChaseCameraArm");
        var head = this.FindChildByName<RigidBody3D>("DriverHead");

        var camDest = head.GlobalPosition + head.LinearVelocity * 2f;

        cam.GlobalPosition = cam.GlobalPosition * .99f + camDest * .01f;



        // grabbed

        if (!Input.IsActionPressed("grab") || !IsInstanceValid(CurrentlyGrabbed)) CurrentlyGrabbed = null;

        if (CurrentlyGrabbed != null)
        {
            CurrentlyGrabbed.AxisLockLinearX = false;
            CurrentlyGrabbed.AxisLockLinearY = false;
            CurrentlyGrabbed.AxisLockAngularZ = false;

            TentacleTarget = CurrentlyGrabbed;
            TentacleTargetTime = 0.1f;

            var distToHead = head.GlobalPosition.DistanceTo(CurrentlyGrabbed.GlobalPosition);
            if (distToHead > GrabRange)
            {
                GD.Print("Got too far away!");
                CurrentlyGrabbed = null;
            }
            else
            {
                var cursorPosition = Picking.PickPlaneAtCursor(this, new Plane(
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0)
                ));

                GD.Print($"cursorPosition={cursorPosition}");
                if (cursorPosition != null)
                {
                    var grabForce = (cursorPosition.Value - CurrentlyGrabbed.GlobalPosition).Normalized() * 160 * CurrentlyGrabbed.Mass / 8;

                    CurrentlyGrabbed.ApplyCentralForce(grabForce);
                    head.ApplyCentralForce(-grabForce);
                }
            }
        }

        // impact detection
        var mb = this.FindChildByName<RigidBody3D>("MainBody");

        var changeInVelocity = PrevVelocity.DistanceTo(mb.LinearVelocity);
        if (InvulnerableTime <= 0 && changeInVelocity > 0.2f && RecentCollision > 0)
        {
            var damage = changeInVelocity * (5f / .3f) / 3;

            GD.Print($"changeInVelocity={changeInVelocity} damage={damage}");

            Health -= damage;
        }

        PrevVelocity = mb.LinearVelocity;
        PrevRotation = mb.Rotation.Z;
        RecentCollision -= (float)delta;
        SlapCooldownLeft -= (float)delta;
        InvulnerableTime -= (float)delta;

        debugInfo += $"{head.LinearVelocity.Length():0.00}\n";

        // drag in the water

        var totalNonHorseMass = this.FindChildrenByType<RigidBody3D>().Where(it => it.Name != "SeaHorse").Select(it => it.Mass).Sum();

        var dragRatio = totalNonHorseMass / OriginalTotalNonHorseMass;
        debugInfo += $"dragRatio={dragRatio}\n";

        foreach (var it in this.FindChildrenByType<RigidBody3D>())
        {
            it.LinearDamp = 0.0f;
            it.LinearDampMode = RigidBody3D.DampMode.Replace;

            var dragForce = -it.LinearVelocity * 15;

            it.ApplyCentralForce(dragForce);

            debugInfo += $" {dragForce.Length():0.00}";
        }

        var debugInfoLabel = GetTree().CurrentScene.FindChildByName<Label>("DebugInfo");
        if (debugInfoLabel != null) debugInfoLabel.Text = debugInfo;

        if (Health <= 0)
        {
            Util.SpawnOneShotParticleSystem(GD.Load<PackedScene>("res://particles/ChariotExplosion.tscn"), this, MainBodyPos);
            var cam2 = this.FindChildByType<Camera3D>();

            var curScene = GetTree().CurrentScene;
            var tree = GetTree();

            cam2.Reparent(curScene);

            Util.StartOneShotTimer(curScene, 3, () =>
            {
                try
                {
                    GD.Print($"Reloading current scene {curScene.SceneFilePath}");
                    tree.ReloadCurrentScene();
                }
                catch (Exception err)
                {
                    GD.PushWarning(err);
                }
            });

            QueueFree();
        }

        // tentacle movement
        var tentacle = this.FindChildByName<Node3D>("RightArmTentacle");
        var targetLength = 1f;

        if (TentacleTarget != null && IsInstanceValid(TentacleTarget))
        {
            tentacle.LookAt(TentacleTarget.GlobalPosition);
            targetLength = tentacle.GlobalPosition.DistanceTo(TentacleTarget.GlobalPosition);
        }
        else
        {
            tentacle.RotationDegrees = new Vector3(-99, 0, 0);
            targetLength = 1;
        }

        var tentacleExtender = this.FindChildByName<Node3D>("RightArmTentacleScale");
        tentacleExtender.Scale = new Vector3(1, 1, tentacleExtender.Scale.Z * .85f + targetLength * .15f);

        TentacleTargetTime -= (float)delta;

        if (TentacleTargetTime <= 0)
        {
            TentacleTarget = null;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);


    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey)
        {
            if (@event.IsActionPressed("exit_game")) GetTree().Quit();
            if (@event.IsActionPressed("cheat_self_destruct") && Default.EnableCheats) Health = 0;
        }

        if (@event is InputEventMouseButton)
        {
            if (@event.IsActionPressed("grab"))
            {
                var picked = Picking.PickObjectAtCursor(this);
                GD.Print($"picked={picked}");
                if (picked is Grabbable)
                {
                    GD.Print("Grabbed something");
                    CurrentlyGrabbed = (Grabbable)picked;
                }
                else if (SlapCooldownLeft <= 0)
                {
                    // maybe slap a shark?
                    var cursorPosition = Picking.PickPlaneAtCursor(this, new Plane(
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(0, 1, 0)
                    ));

                    if (cursorPosition != null)
                    {
                        var head = this.FindChildByName<RigidBody3D>("DriverHead");

                        var nearestShark = GetTree().CurrentScene.FindChildrenByType<AggressiveFish>().MinBy(it => it.GlobalPosition.DistanceTo(cursorPosition.Value));

                        if (nearestShark != null)
                        {
                            if (nearestShark.GlobalPosition.DistanceTo(head.GlobalPosition) <= SlapRange)
                            {
                                GD.Print($"SLAPPED {nearestShark}");
                                SlapCooldownLeft = SlapCooldown;
                                TentacleTarget = nearestShark;
                                TentacleTargetTime = 0.4f;

                                Util.StartOneShotTimer(this, 0.2f, () =>
                                {
                                    nearestShark.Slapped();
                                });
                            }
                            else
                            {
                                GD.Print("Out of SLAP range!");
                            }
                        }
                        else
                        {
                            GD.Print("No shark to slap");
                        }
                    }
                    else
                    {
                        GD.Print("Weird picking error?");
                    }
                }
            }
        }
    }

    public Vector3 MainBodyPos => this.FindChildByName<Node3D>("MainBody").GlobalPosition;
}
