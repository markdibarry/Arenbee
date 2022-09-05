using GameCore.Input;
using GameCore.Items;
using GameCore.Statistics;
using Godot;

namespace GameCore.Actors;

/// <summary>
/// Base character object.
/// </summary>
public abstract partial class Actor : CharacterBody2D, IDamageable
{
    protected Actor()
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
        Equipment = new Equipment(this);
        IFrameController = new IFrameController(this);
        InputHandler = new DummyInputHandler();
    }

    private Node2D _body;
    private Equipment _equipment;
    [Export(PropertyHint.Enum)]
    public ActorType ActorType { get; set; } = ActorType.NPC;
    public string ActorName { get; set; }
    public string ActorId { get; set; }
    public Inventory Inventory { get; set; }
    public Equipment Equipment
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
    public WeaponSlot WeaponSlot { get; private set; }
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
        WeaponSlot.Init(this);
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

    private void OnEquipmentSet(EquipmentSlot slot, Item oldItem, Item newItem)
    {
        oldItem?.RemoveFromStats(Stats);
        newItem?.AddToStats(Stats);
        if (slot.SlotName == EquipSlotName.Weapon)
            WeaponSlot?.SetWeapon(newItem);
    }

    private void SetNodeReferences()
    {
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _body = GetNode<Node2D>("Body");
        BodySprite = _body.GetNode<Sprite2D>("BodySprite");
        BodyShader = (ShaderMaterial)BodySprite.Material;
        WeaponSlot = _body.GetNode<WeaponSlot>("WeaponSlot");
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
