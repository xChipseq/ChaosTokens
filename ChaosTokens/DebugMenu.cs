using System;
using System.Linq;
using ChaosTokens.Modifiers.Effects;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace ChaosTokens;

[RegisterInIl2Cpp]
public class TokenDebugWindow(IntPtr cppPtr) : MonoBehaviour(cppPtr)
{
    private static Rect _windowRect = new(0, 0, 180, 1);
    private static bool _visible = false;

    public void OnGUI()
    {
        if (_visible)
        {
            _windowRect = GUILayout.Window(0, _windowRect, (GUI.WindowFunction)WindowFunction, "Token Dev Window");
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _visible = !_visible;
        }
    }

    public void WindowFunction(int windowID)
    {
        if (!_visible) return;
        
        GUILayout.Label("Abilities");
        if (GUILayout.Button("Reset your cooldown"))
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                ability.ResetCooldownAndOrEffect();
            }
        }
        if (GUILayout.Button("Set cooldown to 0"))
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                ability.SetTimer(0);
            }
        }
        if (GUILayout.Button("Increase ability uses"))
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                ability.IncreaseUses();
            }
        }
        if (GUILayout.Button("Decrease ability uses"))
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                ability.DecreaseUses();
            }
        }
        
        GUILayout.Space(5f);
        GUILayout.Label("Tokens");
        
        if (GUILayout.Button("Get Token"))
        {
            PlayerControl.LocalPlayer.RpcIncreaseTokens(1);
        }
        
        if (GUILayout.Button("Lose A Token"))
        {
            PlayerControl.LocalPlayer.RpcDecreaseTokens(1);
        }
        
        GUILayout.Space(5f);
        GUILayout.Label("Effects");

        foreach (var modifier in ModifierManager.Modifiers.OfType<TokenEffect>())
        {
            if (GUILayout.Button(modifier.ModifierName.Replace("Token ", string.Empty)))
            {
                ChaosTokensRpc.ApplyEffect(PlayerControl.LocalPlayer, modifier.Effect);
            }
        }
                    
        GUI.DragWindow();
    }
}