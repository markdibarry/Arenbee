using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

public abstract class TransitionControllerBase
{
    private LoadingScreen? _loadingScreen;
    private TransitionRequest? _pendingTransition;
    private Transition? _transitionA;
    private Transition? _transitionB;

    public virtual void Update()
    {
        if (_pendingTransition != null)
            _ = TransitionAsync(_pendingTransition);
    }

    public void RequestTransition(TransitionRequest request)
    {
        _pendingTransition = request;
    }

    public async Task TransitionAsync(TransitionRequest request)
    {
        _pendingTransition = null;
        Loader loader = new(request.Paths);
        await TransitionInAsync(request);
        await LoadAsync(loader);
        await request.Callback?.Invoke(loader);
        await TransitionOutAsync(request);
    }

    private async Task LoadAsync(Loader loader)
    {
        loader.ProgressUpdate += OnProgressUpdate;
        await loader.LoadAsync();
        loader.ProgressUpdate -= OnProgressUpdate;
    }

    private void OnProgressUpdate(int progress)
    {
        _loadingScreen?.Update(progress);
    }

    private async Task TransitionInAsync(TransitionRequest request)
    {
        Node target = GetTarget(request.TransitionType);
        // Use Loader Transition
        if (request.TransitionAPath == null)
        {
            _loadingScreen = GDEx.Instantiate<LoadingScreen>(request.LoadingScreenPath);
            target.AddChild(_loadingScreen);
            await _loadingScreen.TransistionFrom();
        }
        // No loader just Transition
        else if (request.TransitionBPath == null)
        {
            _transitionA = GDEx.Instantiate<Transition>(request.TransitionAPath);
            target.AddChild(_transitionA);
            await _transitionA.TransistionFrom();
        }
        // Both loader and transition
        else
        {
            _transitionA = GDEx.Instantiate<Transition>(request.TransitionAPath);
            target.AddChild(_transitionA);
            await _transitionA.TransistionFrom();
            _loadingScreen = GDEx.Instantiate<LoadingScreen>(request.LoadingScreenPath);
            target.AddChild(_loadingScreen);
            await _transitionA.TransitionTo();
            target.RemoveChild(_transitionA);
            _transitionA.QueueFree();
        }
    }

    private async Task TransitionOutAsync(TransitionRequest request)
    {
        Node target = GetTarget(request.TransitionType);
        // Use Loader Transition
        if (request.TransitionAPath == null)
        {
            await _loadingScreen.TransitionTo();
            target.RemoveChild(_loadingScreen);
            _loadingScreen.QueueFree();
        }
        // No loader just Transition
        else if (request.TransitionBPath == null)
        {
            await _transitionA.TransitionTo();
            target.RemoveChild(_transitionA);
            _transitionA.QueueFree();
        }
        // Both loader and transition
        else
        {
            _transitionB = GDEx.Instantiate<Transition>(request.TransitionBPath);
            target.AddChild(_transitionB);
            await _transitionB.TransistionFrom();
            target.RemoveChild(_loadingScreen);
            _loadingScreen.QueueFree();
            await _transitionB.TransitionTo();
            target.RemoveChild(_transitionB);
            _transitionB.QueueFree();
        }
        _transitionA = null;
        _transitionB = null;
        _loadingScreen = null;
    }

    private static Node GetTarget(TransitionType transitionType)
    {
        return transitionType switch
        {
            TransitionType.Game => Locator.Root.Transition,
            TransitionType.Session => Locator.Session?.Transition,
            _ => null
        };
    }
}
