using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI;

public abstract class TransitionControllerBase
{
    public enum TransitionState
    {
        Inactive,
        Started,
        TransitioningIn,
        Loading,
        Loaded,
        RunningCallback,
        CallbackComplete,
        TransitioningOut
    }

    private LoadingScreen _loadingScreen;
    private Loader _loader;
    private TransitionRequest _pendingTransition;
    private Transition _transitionA;
    private Transition _transitionB;
    public TransitionState Status { get; set; }

    public virtual void Update()
    {
        if (Status != TransitionState.Inactive)
            Run();
    }

    public void ChangeScene(TransitionRequest request)
    {
        if (Status != TransitionState.Inactive)
            return;
        Status = TransitionState.Started;
        _pendingTransition = request;
        _loader = new Loader(request.Paths);
    }

    private void Run()
    {
        switch (Status)
        {
            case TransitionState.Started:
                TransitionIn();
                break;
            case TransitionState.Loading:
                Load();
                break;
            case TransitionState.Loaded:
                InvokeCallback();
                break;
            case TransitionState.CallbackComplete:
                TransitionOut();
                break;
        }
    }

    private void Load()
    {
        int progress = _loader.Load();
        _loadingScreen?.Update(progress);
        if (_loader.Status == LoaderStatus.AllLoaded)
            Status = TransitionState.Loaded;
    }

    private async void InvokeCallback()
    {
        Status = TransitionState.RunningCallback;
        await _pendingTransition.Callback?.Invoke(_loader);
        Status = TransitionState.CallbackComplete;
    }

    private async void TransitionIn()
    {
        Status = TransitionState.TransitioningIn;
        var request = _pendingTransition;
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
        Status = TransitionState.Loading;
    }

    private async void TransitionOut()
    {
        Status = TransitionState.TransitioningOut;
        var request = _pendingTransition;
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
        _loader = null;
        _pendingTransition = null;
        Status = TransitionState.Inactive;
    }

    private Node GetTarget(TransitionType transitionType)
    {
        return transitionType switch
        {
            TransitionType.Game => Locator.Root?.Transition,
            TransitionType.Session => Locator.Session?.Transition,
            _ => null
        };
    }
}
