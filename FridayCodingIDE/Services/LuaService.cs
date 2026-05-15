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
            public string Message { get; set; }
        }

        public List<SyntaxError> CheckSyntax(string code)
        {
            var errors = new List<SyntaxError>();
            try
            {
                // MoonSharp's Script object can parse without full execution
                var script = new Script();
                script.DoString(code);
            }
            catch (SyntaxErrorException ex)
            {
                // We could parse the exception message for line numbers if needed
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
            // FNF/Psych Engine commonly used globals/functions
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
