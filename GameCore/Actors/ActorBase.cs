using GameCore.Input;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace GameCore.Actors;

/// <summary>
/// Base character object.
/// </summary>
public abstract partial class ActorBase : CharacterBody2D, IDamageable
{
    protected ActorBase()
    {
        UpDirection = Vector2.Up;
        Acceleration = 600;
        Friction = 600;
        GroundedGravity = 0.05;
        WalkSpeed = 50;
        Direction = new Vector2(1, 1);
        Stats = new Stats(this);
        ApplyDefaultStats();
        Inventory = new Inventory();
        Equipment = new EquipmentBase(this);
        IFrameController = new IFrameController(this);
        InputHandler = new DummyInputHandler();
    }

    private Node2D _body;
    private EquipmentBase _equipment;
    [Export(PropertyHint.Enum)]
    public ActorType ActorType { get; set; } = ActorType.NPC;
    public string ActorName { get; set; }
    public string ActorId { get; set; }
    public Inventory Inventory { get; set; }
    public EquipmentBase Equipment
    {
        get => _equipment;
        private set
        {
            if (_equipment != null)
                _equipment.EquipmentSet -= OnEquipmentSet;
            _equipment = value;
            if (_equipment != null)
                _equipment.EquipmentSet += OnEquipmentSet;
        }
    }
    public HoldItemControllerBase HoldItemController { get; private set; }
    public AreaBoxContainer HurtBoxes { get; private set; }
    public AreaBoxContainer HitBoxes { get; private set; }
    public ShaderMaterial BodyShader { get; set; }
    public int ShaderCycleStart
    {
        get => (int)BodyShader.GetShaderParameter("cycle_start");
        set => BodyShader.SetShaderParameter("cycle_start", value);
    }
    public int ShaderCycleEnd
    {
        get => (int)BodyShader.GetShaderParameter("cycle_end");
        set => BodyShader.SetShaderParameter("cycle_end", value);
    }
    public float ShaderSpeed
    {
        get => (float)BodyShader.GetShaderParameter("speed");
        set => BodyShader.SetShaderParameter("speed", value);
    }

    public override void _Ready()
    {
        SetNodeReferences();
        _floatPosition = GlobalPosition;
        InitBoxes();
        InitMovement();
        InitState();
        Init();
        HoldItemController.Init(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = _floatPosition;
        _move = Vector2.Zero;
        Stats.Process(delta);
        StateController.UpdateStates(delta);
        IFrameController.Process(delta);
        HandleMove(delta);
    }

    public virtual void Init() { }

    public void InitBoxes()
    {
        SetHitBoxes();
        foreach (HurtBox hurtbox in HurtBoxes.GetChildren())
            hurtbox.AreaEntered += Stats.OnHurtBoxEntered;
    }

    protected virtual void SetHitBoxes() { }

    private void OnEquipmentSet(EquipmentSlotBase slot, ItemBase oldItem, ItemBase newItem)
    {
        oldItem?.RemoveFromStats(Stats);
        newItem?.AddToStats(Stats);
        HoldItemController?.SetHoldItem(oldItem, newItem);
    }

    private void SetNodeReferences()
    {
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _body = GetNode<Node2D>("Body");
        BodySprite = _body.GetNode<Sprite2D>("BodySprite");
        BodyShader = (ShaderMaterial)BodySprite.Material;
        HoldItemController = _body.GetNode<HoldItemControllerBase>("HoldItems");
        HurtBoxes = BodySprite.GetNode<AreaBoxContainer>("HurtBoxes");
        HitBoxes = BodySprite.GetNode<AreaBoxContainer>("HitBoxes");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}

public enum ActorType
{
    Player,
    Enemy,
    NPC
}
