using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.Framework.GUI.Dialogs;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class GUIController : CanvasLayer
    {
        public GUIController()
        {
            _guiLayers = new List<GUILayer>();
            _dialogScene = GD.Load<PackedScene>(Dialog.GetScenePath());
        }

        private GUILayer _closeLayerRequested;
        private Action _closeLayerCallback;
        private readonly PackedScene _dialogScene;
        private readonly List<GUILayer> _guiLayers;
        private GUILayer _currentGUILayer;
        private bool _closeAll;
        public bool MenuActive { get; set; }
        public bool DialogActive { get; set; }
        public bool GUIActive => MenuActive || DialogActive;
        public delegate void MenuStatusChangedHandler(GUIController guiController);
        public event MenuStatusChangedHandler GUIStatusChanged;

        public void HandleInput(GUIInputHandler menuInput, float delta)
        {
            _currentGUILayer?.HandleInput(menuInput, delta);
        }

        public override void _Process(float delta)
        {
            if (_closeLayerRequested != null)
            {
                CloseGUILayer(_closeLayerRequested, _closeLayerCallback);
                if (_closeAll)
                    CloseAll();
            }
        }

        public void CloseAll()
        {
            if (_currentGUILayer == null)
            {
                _closeAll = false;
                return;
            }
            _closeAll = true;
            _closeLayerRequested = _currentGUILayer;
        }

        public void CloseGUILayer(GUILayer layerToClose, Action closeCallback = null)
        {
            _closeLayerRequested = null;
            _closeLayerCallback = null;
            if (!_guiLayers.Contains(layerToClose))
                return;
            layerToClose.RequestedClose -= OnRequestedClose;
            if (layerToClose is Dialog dialog)
                dialog.OptionBoxRequested -= OnOptionBoxRequested;
            RemoveChild(layerToClose);
            layerToClose.QueueFree();
            _guiLayers.Remove(layerToClose);
            UpdateCurrentGUI();
            closeCallback?.Invoke();
        }

        public bool IsActive(string guiLayerName)
        {
            return _guiLayers.Any(x => x.NameId == guiLayerName);
        }

        public void OpenDialog(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            Dialog newDialog = _dialogScene.Instantiate<Dialog>();
            newDialog.RequestedClose += OnRequestedClose;
            newDialog.OptionBoxRequested += OnOptionBoxRequested;
            AddChild(newDialog);
            _guiLayers.Add(newDialog);
            UpdateCurrentGUI();
            newDialog.StartDialog(path);
        }

        public async Task OpenMenuAsync(PackedScene menuScene)
        {
            Menu newMenu = menuScene.Instantiate<Menu>();
            await OpenMenuAsync(newMenu);
        }

        public async Task OpenMenuAsync(Menu menu)
        {
            menu.RequestedClose += OnRequestedClose;
            AddChild(menu);
            _guiLayers.Add(menu);
            UpdateCurrentGUI();
            await menu.InitAsync();
        }

        public void OnRequestedClose(GUILayer guiLayer, Action callback)
        {
            _closeLayerRequested = guiLayer;
            _closeLayerCallback = callback;
        }

        public void OnOptionBoxRequested(Menu menu)
        {
            OpenMenuAsync(menu);
        }

        private void UpdateCurrentGUI()
        {
            _currentGUILayer = _guiLayers.LastOrDefault();
            MenuActive = _guiLayers.Any(x => x is Menu);
            DialogActive = _guiLayers.Any(x => x is Dialog);
            GUIStatusChanged?.Invoke(this);
        }
    }
}