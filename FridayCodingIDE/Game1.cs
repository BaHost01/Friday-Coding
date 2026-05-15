using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FridayCodingIDE.Services;
using FridayCodingIDE.UI;
using System.IO;
using System;

namespace FridayCodingIDE;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ProjectManager _projectManager;
    private ScreenManager _screenManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _projectManager = new ProjectManager();
    }

    protected override void Initialize()
    {
        _screenManager = new ScreenManager(GraphicsDevice);
        
        // For testing: initialize or load a project in the parent directory
        string workspacePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        if (!_projectManager.LoadProject(workspacePath))
        {
            _projectManager.InitializeNewProject(workspacePath, "Default FNF Mod Project");
        }

        // Start with the Intro Loading Screen
        _screenManager.ChangeScreen(new IntroLoadingScreen(this, _screenManager));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _screenManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _screenManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
