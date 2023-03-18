using System;
using System.Threading.Tasks;

namespace GameCore.GUI;

public class TransitionRequest
{
    public TransitionRequest(
        string loadingScreenPath,
        TransitionType transitionType,
        string[] paths,
        Func<Loader, Task> callback)
        : this(loadingScreenPath, transitionType, string.Empty, string.Empty, paths, callback)
    {
    }

    public TransitionRequest(
        TransitionType transitionType,
        string transitionA,
        string[] paths,
        Func<Loader, Task> callback)
        : this(string.Empty, transitionType, transitionA, string.Empty, paths, callback)
    {
    }

    public TransitionRequest(
        string loadingScreenPath,
        TransitionType transitionType,
        string transitionA,
        string transitionB,
        string[] paths,
        Func<Loader, Task> callback)
    {
        LoadingScreenPath = loadingScreenPath;
        TransitionType = transitionType;
        TransitionAPath = transitionA;
        TransitionBPath = transitionB;
        Paths = paths;
        Callback = callback;
    }

    public string TransitionAPath { get; set; }
    public string TransitionBPath { get; set; }
    public string LoadingScreenPath { get; set; }
    public string[] Paths { get; set; }
    public Func<Loader, Task> Callback { get; set; }
    public TransitionType TransitionType { get; set; }
}

public enum TransitionType
{
    Game,
    Session
}
