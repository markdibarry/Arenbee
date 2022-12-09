using System;
using System.Threading.Tasks;
using Godot;

namespace GameCore.GUI;

public interface IMenu
{
    Task CloseSubMenuAsync(Type cascadeTo = null, bool preventAnimation = false, object data = null);
    Task OpenSubMenuAsync(string path, bool preventAnimation = false, object data = null);
    Task OpenSubMenuAsync(PackedScene packedScene, bool preventAnimation = false, object data = null);
}
