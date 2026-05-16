using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace FridayCodingIDE.Services
{
    public class LuaService
    {
        public class SyntaxError
        {
            public int Line { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public void ExecuteScript(string code, Action<string> onPrint)
        {
            try
            {
                var script = new Script();
                
                // Register FNF-like functions
                script.Globals["debugPrint"] = (Action<string>)((msg) => onPrint?.Invoke(msg));
                
                // Add common placeholders for Psych Engine functions to prevent crashes
                script.Globals["makeLuaSprite"] = (Action<string, string, float, float>)((t, p, x, y) => onPrint?.Invoke($"[SPRITE] Created {t} from {p} at ({x}, {y})"));
                script.Globals["addLuaSprite"] = (Action<string, bool>)((t, f) => onPrint?.Invoke($"[SPRITE] Added {t}"));
                script.Globals["scaleObject"] = (Action<string, float, float>)((t, x, y) => { });
                script.Globals["setProperty"] = (Action<string, object>)((p, v) => { });
                script.Globals["setObjectCamera"] = (Action<string, string>)((t, c) => { });

                script.DoString(code);
            }
            catch (Exception ex)
            {
                onPrint?.Invoke($"ERROR: {ex.Message}");
            }
        }

        public List<SyntaxError> CheckSyntax(string code)
        {
            var errors = new List<SyntaxError>();
            try
            {
                var script = new Script();
                script.DoString(code);
            }
            catch (SyntaxErrorException ex)
            {
                errors.Add(new SyntaxError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                errors.Add(new SyntaxError { Message = ex.Message });
            }
            return errors;
        }

        public List<string> GetAutocomplete(string partialWord)
        {
            var symbols = new List<string>
            {
                "function onCreate()",
                "function onUpdate(elapsed)",
                "function onBeatHit()",
                "function onStepHit()",
                "addLuaSprite",
                "makeLuaSprite",
                "setObjectCamera",
                "scaleObject",
                "playAnim",
                "characterPlayAnim",
                "getProperty",
                "setProperty",
                "debugPrint",
                "close"
            };

            return symbols.FindAll(s => s.StartsWith(partialWord, StringComparison.OrdinalIgnoreCase));
        }
    }
}