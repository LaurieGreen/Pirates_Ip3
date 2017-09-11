using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;
using System.IO;
using Module_Persistence;
using Module_Basic_Entities;
using System.Threading;

namespace SimpleGame
{
class GameplayScreen : GameScreen
{
//Animation
float timer = 0f;
float interval = 90f;
int currentFrame = 1;
Rectangle sourceRect;
Vector2 origin;

//Graphics
Map map;
public ContentManager content;
SpriteBatch spriteBatch;
Color backColor = Color.White;
SpriteFont font;
Texture2D backgroundTexture;
Rectangle viewportRect;

//Enemies
List<Enemy> bossList;
List<Enemy> pirateList;
Texture2D pirateTexture;
TimeSpan enemySpawnTime;
TimeSpan previousSpawnTime;
Random random;

//Player
Player playerSprite;
GameObject arm;
PlayerState playerState;
public enum PlayerState { Normal, Bossfight };
GunState gunState;
public enum GunState { Pistol, Shotgun, SMG, M16, MP5, RPG, Laser, GatlingGun };
public int gunDamage;
public int progress;

//Projectiles
Texture2D projectileTexture;
List<Projectile> projectiles;
TimeSpan fireTime;
TimeSpan previousFireTime;


//Camera
private float cameraPosition;
public float cameraPositionYAxis;
private SpriteEffects flip = SpriteEffects.None;

//HUD
GameObject health;
GameObject gunwheellarge;
GameObject gunwheelsmall;
GameObject gunwheelsmall2;
GameObject browser;
//GameObject progressbar;
Texture2D progressbar;


//Other misc
public Vector2 startPosition1;
PhysicsSimulator simulator = new PhysicsSimulator(new Vector2(0, 2000));
private Texture2D cursorTex;
private Vector2 cursorPos;

public GameplayScreen()
{
startPosition1 = new Vector2(100, 320);
progress = 0;
}
public override void LoadContent()
{
if (content == null)
content = new ContentManager(ScreenManager.Game.Services, "Content");

// Create a new SpriteBatch, which can be used to draw textures.
spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
font = content.Load<SpriteFont>("Fonts/gamefont");
backgroundTexture = content.Load<Texture2D>("snap");
viewportRect = new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height);

cursorTex = content.Load<Texture2D>("cursor");
map = new Map(content);
arm = new GameObject();
arm.currentTexture = content.Load<Texture2D>("Guns/pistol");

projectiles = new List<Projectile>();
projectileTexture = content.Load<Texture2D>("Guns/Projectile");

pirateList = new List<Enemy>();
pirateTexture = content.Load<Texture2D>("pirate");

bossList = new List<Enemy>();

health = new GameObject();
health.currentTexture = content.Load<Texture2D>("HUD/healthfull");
health.position = new Vector2(cameraPosition + 1140, cameraPositionYAxis + 670);

gunwheellarge = new GameObject();
gunwheellarge.currentTexture = content.Load<Texture2D>("HUD/largecircle");
gunwheellarge.position = new Vector2(cameraPosition, cameraPositionYAxis + 690);

gunwheelsmall = new GameObject();
gunwheelsmall.currentTexture = content.Load<Texture2D>("HUD/normalcircle");
gunwheelsmall.position = new Vector2(cameraPosition + 130, cameraPositionYAxis + 710);

gunwheelsmall2 = new GameObject();
gunwheelsmall2.currentTexture = content.Load<Texture2D>("HUD/normalcircle");
gunwheelsmall2.position = new Vector2(cameraPosition + 240, cameraPositionYAxis + 710);

browser = new GameObject();
browser.currentTexture = content.Load<Texture2D>("HUD/tophud");
browser.position = new Vector2(cameraPosition, cameraPositionYAxis);

progressbar = content.Load<Texture2D>("bar");

//MediaPlayer.IsRepeating = true;
//MediaPlayer.Play(content.Load<Song>("piratesong"));

// Set the time keepers to zero
previousSpawnTime = TimeSpan.Zero;
// Used to determine how fast enemy respawns
enemySpawnTime = TimeSpan.FromSeconds(2.0f);

playerSprite = new Player();
playerSprite.Position = startPosition1;
playerSprite.Load("assetname", content, simulator);
playerSprite.Geom.OnCollision += OnCollision;
playerSprite.isidle = true;

LoadEnemies();

KeyboardState newState = Keyboard.GetState();
AddTileBody();
}

public void LoadEnemies()
{
for (int i = 0; i < map.enemy1spawnList.Count(); i++)
{
AddEnemy(spriteBatch.GraphicsDevice.Viewport, map.enemy1spawnList[i].Position);
}
}
public void AddTileBody()
{
for (int i = 0; i < map.tileList.Count(); i++)
{
map.tileList[i].body = BodyFactory.Instance.CreateRectangleBody(32, 32, 10.0f);
map.tileList[i].geom = GeomFactory.Instance.CreateRectangleGeom(map.tileList[i].Body, 32, 10.0f);
map.tileList[i].body.IsStatic = true;
map.tileList[i].body.Position = new Vector2(map.tileList[i].Position.X + 16, map.tileList[i].Position.Y + 7);
map.tileList[i].body.Tag = this;
map.tileList[i].geom.Tag = this;
simulator.Add(map.tileList[i].body);
simulator.Add(map.tileList[i].geom);
}
}
public bool OnCollision(Geom g1, Geom g2, ContactList contactList)
{
Player s1 = (Player)g1.Tag;

if (g2.Tag.GetType() == typeof(Tile))
{
Tile tile = (Tile)g2.Tag;
}

return true;
}
public override void UnloadContent()
{
content.Unload();
}
public void Killplayer()
{
playerSprite.Position = startPosition1;
playerSprite.health = 100.0f;

for (int i = 0; i < pirateList.Count(); i++)
{
pirateList[i].Active = false;
}
for (int i = 0; i < projectiles.Count(); i++)
{
projectiles[i].Active = false;
}
LoadEnemies();
}
public void Doswitch(PlayerState playerState, GunState gunState, GameTime gameTime, Viewport viewport)
{
switch (playerState)
{
case PlayerState.Normal:
UpdateEnemies(gameTime, spriteBatch.GraphicsDevice.Viewport);
ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
playerSprite.isinbossfight = false;
break;
case PlayerState.Bossfight:
AddBoss(ScreenManager.GraphicsDevice.Viewport, playerSprite.Position);
for (int i = 0; i < bossList.Count(); i++)
{
bossList[i].Update(gameTime, playerSprite);

if (bossList[i].Position.X > playerSprite.Position.X)
{
bossList[i].Body.LinearVelocity.X = -bossList[i].moveSpeed;
}
else
{
bossList[i].Body.LinearVelocity.X = 0f;
}
}
break;
}
switch (gunState)
{
case GunState.Pistol:
arm.currentTexture = content.Load<Texture2D>("Guns/pistol");
fireTime = TimeSpan.FromSeconds(1.0f);
gunDamage = 1;
//gunRateofFire = 2;
break;
case GunState.Shotgun:
arm.currentTexture = content.Load<Texture2D>("Guns/shotgun");
fireTime = TimeSpan.FromSeconds(1.5f);
gunDamage = 2;
//gunRateofFire = 1;
break;
case GunState.SMG:
arm.currentTexture = content.Load<Texture2D>("Guns/smg");
fireTime = TimeSpan.FromSeconds((0.5f));
gunDamage = 2;
//gunRateofFire = 3;
break;
case GunState.M16:
arm.currentTexture = content.Load<Texture2D>("Guns/m16");
fireTime = TimeSpan.FromSeconds((0.75f));
gunDamage = 3;
//gunRateofFire = 4;
break;
case GunState.MP5:
arm.currentTexture = content.Load<Texture2D>("Guns/mp5");
fireTime = TimeSpan.FromSeconds((0.5f));
gunDamage = 3;
//gunRateofFire = 3;
break;
case GunState.RPG:
arm.currentTexture = content.Load<Texture2D>("Guns/RPG");
fireTime = TimeSpan.FromSeconds(1.0f);
gunDamage = 4;
//gunRateofFire = 2;
break;
case GunState.Laser:
arm.currentTexture = content.Load<Texture2D>("Guns/laser");
fireTime = TimeSpan.FromSeconds((0.75f));
//gunDamage = 4;
break;
case GunState.GatlingGun:
arm.currentTexture = content.Load<Texture2D>("Guns/gatlinggun");
fireTime = TimeSpan.FromSeconds((0.625f));
//gunDamage = 4;
//gunRateofFire = 5;
break;
}
}
public void LoadLevel(GameTime gameTime)
{
for (int i = 0; i < pirateList.Count(); i++)
{
pirateList[i].Active = false;
}
for (int i = 0; i < map.tileList.Count(); i++)
{
map.tileList[i].Active = false;
}
//for (int i = 0; i < map.endList.Count(); i++)
//{
// //map.endList[i].Update(gameTime);
// //map.endList[i].body.Dispose();
// //map.endList[i].geom.Dispose();
// //map.endList.RemoveAt(i); 
//}
UpdateTiles(gameTime);
map.endList.Clear();
map.tileList.Clear();
map.enemy1spawnList.Clear();
map.currentlevel++;
map.LoadLevel(content);
playerSprite.Position = startPosition1;
AddTileBody();
LoadEnemies();
}

public void UpdateTiles(GameTime gameTime)
{
for (int i = map.tileList.Count - 1; i >= 0; i--)
{
map.tileList[i].Update(gameTime);
if (map.tileList[i].Active == false)
{
map.tileList[i].Body.Dispose();
map.tileList[i].Geom.Dispose();
map.tileList.RemoveAt(i);
}
}
for (int i = 0; i < map.tileList.Count(); i++)
{
map.tileList[i].Update(gameTime);
}
//for (int i = 0; i < map.endList.Count(); i++)
//{
// map.endList[i].Update(gameTime);
//}
}

public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
{
if (otherScreenHasFocus != true)
{
simulator.Update(gameTime.ElapsedGameTime.Milliseconds * .001f);
playerSprite.Update(gameTime);
MouseState mouseState = Mouse.GetState();

cursorPos = new Vector2(mouseState.X + cameraPosition, mouseState.Y + cameraPositionYAxis);
Doswitch(playerState, gunState, gameTime, spriteBatch.GraphicsDevice.Viewport);
progress = (int)MathHelper.Clamp(playerSprite.Position.X, 0, (map.levelWidth * map.textureSize));

//if ((playerSprite.Position.X >= 2000) && (playerState == PlayerState.Normal))
//{
// for (int i = pirateList.Count() - 1; i >= 0; i--)
// {
// pirateList[i].Active = false;
// }
// UpdateEnemies(gameTime, ScreenManager.GraphicsDevice.Viewport);
// playerState = PlayerState.Bossfight;
//}
map.Update(gameTime);
UpdateTiles(gameTime);
UpdateCollisions(gameTime);
UpdateProjectiles(gameTime, spriteBatch.GraphicsDevice.Viewport);
UpdateHUD();

if (playerSprite.nukes == 3)
{
playerSprite.canPickupNuke = false;
}


if (flip == SpriteEffects.FlipHorizontally)
arm.position = new Vector2(playerSprite.Position.X - 5, playerSprite.Position.Y + 10);
else
arm.position = new Vector2(playerSprite.Position.X + 5, playerSprite.Position.Y - 10);

if (gameTime.TotalGameTime - previousFireTime > fireTime)
{
previousFireTime = gameTime.TotalGameTime;
playerSprite.canshoot = true;
}
if (playerSprite.Body.LinearVelocity.X == 0.0f)
{
playerSprite.isidle = true;
}
else
{ //Increase the timer by the number of milliseconds since update was last called
timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
//Check the timer is more than the chosen interval
if (timer > interval)
{
//Show the next frame
currentFrame++;
//Reset the timer
timer = 0f;
}
//If we are on the last frame, reset back to the one before the first frame (because currentframe++ is called next so the next frame will be 1!)
if (currentFrame == 6)
{
currentFrame = 0;
}
sourceRect = new Rectangle(currentFrame * playerSprite.spriteWidth, 0, playerSprite.spriteWidth, playerSprite.spriteHeight);
origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
}
}
}

public void UpdateHUD()
{
if ((playerSprite.health <= 100f) && (playerSprite.health > 75f))
{
health.currentTexture = content.Load<Texture2D>("HUD/healthfull");
}
else if ((playerSprite.health < 50f) && (playerSprite.health > 25f))
{
health.currentTexture = content.Load<Texture2D>("HUD/healthhalf");
}
else if ((playerSprite.health < 15f) && (playerSprite.health > 0f))
{
health.currentTexture = content.Load<Texture2D>("HUD/healthlow");
}
health.position = new Vector2(cameraPosition + 1140, cameraPositionYAxis + 670);
gunwheellarge.position = new Vector2(cameraPosition, cameraPositionYAxis + 720);
gunwheelsmall.position = new Vector2(cameraPosition + 130, cameraPositionYAxis + 740);
gunwheelsmall2.position = new Vector2(cameraPosition + 240, cameraPositionYAxis + 740);
browser.position = new Vector2(cameraPosition, cameraPositionYAxis);
//progressbar.position = new Vector2(cameraPosition, cameraPositionYAxis);
}

public override void HandleInput(InputState input)
{
if (input == null)
throw new ArgumentNullException("input");

int playerIndex = (int)ControllingPlayer.Value;
KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

if (input.IsPauseGame(ControllingPlayer))
{
ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
}
else
{
Vector2 movement = Vector2.Zero;
// Get input state.
MouseState mouseState = Mouse.GetState();
//Arm rotation
float x = (mouseState.X + cameraPosition) - playerSprite.Position.X;
float y = (mouseState.Y + cameraPositionYAxis) - playerSprite.Position.Y;
arm.rotation = (float)Math.Atan2(x, -y);
if (flip == SpriteEffects.FlipVertically) //Facing right
{
//If we try to aim behind our head then flip the
//character around so he doesn't break his arm!
if (arm.rotation < 0)
flip = SpriteEffects.None;
}
else //Facing left
{
//Once again, if we try to aim behind us then
//flip our character.
if (arm.rotation > 0)
flip = SpriteEffects.FlipVertically;
//If we're not rotating our arm, default it to
//aim the same direction we're facing.

}
if ((playerSprite.Body.LinearVelocity.Y < 0.001) && (playerSprite.Body.LinearVelocity.Y > -0.001))
{
playerSprite.canjump = true;
playerSprite.isjumping = false;
}
if ((keyboardState.IsKeyDown(Keys.W)) && playerSprite.canjump == true)
{
playerSprite.Body.ApplyForce(new Vector2(0.0f, -playerSprite.jumpSpeed));
//MediaPlayer.Play(content.Load<Song>("PlayerJump"));
playerSprite.canjump = false;
playerSprite.isjumping = true;
}
else if (keyboardState.IsKeyDown(Keys.A))
{
playerSprite.isidle = false;
playerSprite.isrunningright = false;
playerSprite.isfacingright = false;
playerSprite.Body.LinearVelocity.X = -playerSprite.moveSpeed;
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Run/runleft");
if ((mouseState.LeftButton == ButtonState.Pressed) && (playerSprite.canshoot == true))
{
AddProjectile(playerSprite.Position - new Vector2(playerSprite.spriteWidth / 2));
playerSprite.canshoot = false;
}
playerSprite.isrunningleft = true;
playerSprite.isfacingleft = true;
}
else if (keyboardState.IsKeyDown(Keys.D))
{
playerSprite.isidle = false;
playerSprite.isrunningleft = false;
playerSprite.isfacingleft = false;
playerSprite.Body.LinearVelocity.X = playerSprite.moveSpeed;
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Run/runright");
if ((mouseState.LeftButton == ButtonState.Pressed) && (playerSprite.canshoot == true))
{
AddProjectile(playerSprite.Position + new Vector2(playerSprite.spriteWidth / 2));
playerSprite.canshoot = false;
}
playerSprite.isrunningright = true;
playerSprite.isfacingright = true;
}
else if ((mouseState.LeftButton == ButtonState.Pressed) && (playerSprite.canshoot == true))
{
AddProjectile(playerSprite.Position + new Vector2(playerSprite.spriteWidth / 2));
//playerSprite.gun.clipBulletsLeft;
playerSprite.canshoot = false;
}
else if (keyboardState.IsKeyDown(Keys.Space))
{
for (int i = 0; i < pirateList.Count(); i++)
{
if (pirateList[i].Position.X <= (cameraPosition + (spriteBatch.GraphicsDevice.Viewport.Width)) &&
(pirateList[i].Position.X >= (cameraPosition)))

pirateList[i].Active = false;
}
}
else
{
playerSprite.isidle = true;
playerSprite.isrunningleft = false;
playerSprite.isrunningright = false;
playerSprite.Body.LinearVelocity.X = 0.0f;
if (playerSprite.isfacingright == true)
{
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/rightidle");
}
else if (playerSprite.isfacingleft == true)
{
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/leftidle");
}
}
if (playerSprite.isjumping == true && (keyboardState.IsKeyDown(Keys.D)))
{
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/rightidle");
}
if (playerSprite.isjumping == true && (keyboardState.IsKeyDown(Keys.A)))
{
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/leftidle");
}
if (playerSprite.isjumping == true)
{
playerSprite.currentTexture = content.Load<Texture2D>("Smaller Sheets/Idle/rightidle");
}
if (keyboardState.IsKeyDown(Keys.D1))
{
gunState = GunState.Pistol;
}
if (keyboardState.IsKeyDown(Keys.D2))
{
gunState = GunState.Shotgun;
}
if (keyboardState.IsKeyDown(Keys.D3))
{
gunState = GunState.SMG;
}
if (keyboardState.IsKeyDown(Keys.D4))
{
gunState = GunState.M16;
}
if (keyboardState.IsKeyDown(Keys.D5))
{
gunState = GunState.MP5;
}
}
}
private void UpdateCollisions(GameTime gameTime)
{
{
Rectangle rectangle1;
Rectangle rectangle2;

if (playerSprite.isrunningleft == true || playerSprite.isrunningright == true)
rectangle1 = new Rectangle((int)playerSprite.Position.X - ((playerSprite.currentTexture.Width / 6) /2), (int)playerSprite.Position.Y, (playerSprite.currentTexture.Width / 6) * 2, playerSprite.CurrentTexture.Height);
//THIS IS WHAT IS APPLIED BY ELSE if (playerSprite.isidle == true || playerSprite.isjumping == true)
else
rectangle1 = new Rectangle((int)playerSprite.Position.X - (playerSprite.currentTexture.Width / 2), (int)playerSprite.Position.Y, playerSprite.currentTexture.Width * 2, playerSprite.CurrentTexture.Height * 2);
//else if (playerSprite.isjumping == true)
// rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width / 6, playerSprite.CurrentTexture.Height);
//else
// rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width / 6, playerSprite.CurrentTexture.Height);

//Player Vs Enemies
for (int i = 0; i < pirateList.Count; i++)
{
rectangle2 = new Rectangle((int)pirateList[i].Position.X - (pirateList[i].currentTexture.Width / 2), (int)pirateList[i].Position.Y, pirateList[i].currentTexture.Width * 2, pirateList[i].currentTexture.Height);
// Determine if the two objects collided with each
// other
if (rectangle1.Intersects(rectangle2))
{
// Subtract the health from the player based on
// the enemy damage
playerSprite.health -= 0.1f;
if (playerSprite.health <= 0)
{
Killplayer();
}

// Since the enemy collided with the player
// destroy it
//pirateList[i].Health = 0;
}
}
// Projectile vs Enemy Collision
for (int i = 0; i < projectiles.Count; i++)
{
for (int j = 0; j < pirateList.Count(); j++)
{
// Create the rectangles we need to determine if we collided with each other
rectangle1 = new Rectangle((int)projectiles[i].Position.X +
projectiles[i].CurrentTexture.Width / 2, (int)projectiles[i].Position.Y +
projectiles[i].CurrentTexture.Height / 2, projectiles[i].CurrentTexture.Width, projectiles[i].CurrentTexture.Height);

rectangle2 = new Rectangle((int)pirateList[j].Position.X - pirateList[j].spriteWidth,
(int)pirateList[j].Position.Y - pirateList[j].spriteHeight,
pirateList[j].spriteWidth, pirateList[j].spriteHeight);

// Determine if the two objects collided with each other
if (rectangle1.Intersects(rectangle2))
{
pirateList[j].health = pirateList[j].health - gunDamage;
projectiles[i].Active = false;
}
}
}
if (playerState == PlayerState.Bossfight)
{
// Projectile vs Enemy Collision
for (int i = 0; i < projectiles.Count; i++)
{
for (int j = 0; j < bossList.Count(); j++)
{
// Create the rectangles we need to determine if we collided with each other
rectangle1 = new Rectangle((int)projectiles[i].Position.X +
projectiles[i].CurrentTexture.Width / 2, (int)projectiles[i].Position.Y +
projectiles[i].CurrentTexture.Height / 2, projectiles[i].currentTexture.Width, projectiles[i].currentTexture.Height);

rectangle2 = new Rectangle((int)bossList[j].Position.X - bossList[j].spriteWidth,
(int)bossList[j].Position.Y - bossList[j].spriteHeight,
bossList[j].spriteWidth, bossList[j].spriteHeight);

// Determine if the two objects collided with each other
if (rectangle1.Intersects(rectangle2))
{
bossList[j].health = bossList[j].health - 1.0f;
projectiles[i].Active = false;
}
}
}
}
// Projectiles vs Tiles
for (int i = 0; i < projectiles.Count; i++)
{
for (int j = 0; j < map.tileList.Count(); j++)
{
// Create the rectangles we need to determine if we collided with each other
rectangle1 = new Rectangle((int)projectiles[i].Position.X +
projectiles[i].CurrentTexture.Width / 2, (int)projectiles[i].Position.Y +
projectiles[i].CurrentTexture.Height / 2, projectiles[i].currentTexture.Width, projectiles[i].currentTexture.Height);

rectangle2 = new Rectangle((int)map.tileList[j].Position.X + 16 - map.textureSize,
(int)map.tileList[j].Position.Y + 7 - map.textureSize,
map.textureSize, map.textureSize);

// Determine if the two objects collided with each other
if (rectangle1.Intersects(rectangle2))
{
projectiles[i].Active = false;
}
}
}
// Players vs Tiles
for (int i = 0; i < map.killList.Count(); i++)
{
if (playerSprite.isrunningleft == true || playerSprite.isrunningright == true)
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width / 6, playerSprite.currentTexture.Height);
else if (playerSprite.isjumping == true)
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width, playerSprite.currentTexture.Height);
else
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width, playerSprite.currentTexture.Height);

rectangle2 = new Rectangle((int)map.killList[i].Position.X, (int)map.killList[i].Position.Y, map.textureSize, map.textureSize);

if (rectangle1.Intersects(rectangle2))
{
Killplayer();
}
}
// Players vs Tiles
for (int i = 0; i < map.endList.Count(); i++)
{
if (playerSprite.isrunningleft == true || playerSprite.isrunningright == true)
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width / 6, playerSprite.currentTexture.Height);
else if (playerSprite.isjumping == true)
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width, playerSprite.currentTexture.Height);
else
rectangle1 = new Rectangle((int)playerSprite.Position.X, (int)playerSprite.Position.Y, playerSprite.currentTexture.Width, playerSprite.currentTexture.Height);

rectangle2 = new Rectangle((int)map.endList[i].Position.X, (int)map.endList[i].Position.Y, map.textureSize, map.textureSize);

if (rectangle1.Intersects(rectangle2))
{
LoadLevel(gameTime);
}
}
}
}
public void AddBoss(Viewport viewport, Vector2 position)
{
if (playerSprite.isinbossfight == false)
{
Enemy enemy = new Enemy();
enemy.currentTexture = content.Load<Texture2D>("facebook");
enemy.Load("facebook", content, simulator);
enemy.Position = new Vector2(viewport.Width + playerSprite.Position.X + 200, playerSprite.Position.Y - 100);
playerSprite.isinbossfight = true;
bossList.Add(enemy);
}
}
public void AddEnemy(Viewport viewport, Vector2 position)
{
random = new Random();
Enemy enemy = new Enemy();
enemy.Load("pirate", content, simulator);
enemy.Position = position;
enemy.initialposition = position.X;
pirateList.Add(enemy);
}
public void AddProjectile(Vector2 position)
{
Projectile projectile = new Projectile();
projectile.Load("Guns/Projectile", content, simulator);
if (flip == SpriteEffects.FlipVertically) //Facing right
{
float armCos = (float)Math.Cos(arm.rotation - MathHelper.PiOver2);
float armSin = (float)Math.Sin(arm.rotation - MathHelper.PiOver2);
projectile.Position = new Vector2(arm.position.X + arm.currentTexture.Width * armCos, arm.position.Y + 42 * armSin);
projectile.movespeed = new Vector2((float)Math.Cos(arm.rotation - MathHelper.PiOver2), (float)Math.Sin(arm.rotation - MathHelper.PiOver2)) * 15.0f;
}
else //Facing left
{
float armCos = (float)Math.Cos(arm.rotation + MathHelper.PiOver2);
float armSin = (float)Math.Sin(arm.rotation + MathHelper.PiOver2);
projectile.Position = new Vector2(arm.position.X - arm.currentTexture.Width * armCos, arm.position.Y - 42 * armSin);
projectile.movespeed = new Vector2(-armCos, -armSin) * 15.0f;
}
projectiles.Add(projectile);
}

public void UpdateEnemies(GameTime gameTime, Viewport viewport)
{
for (int i = pirateList.Count() - 1; i >= 0; i--)
{
pirateList[i].Update(gameTime, playerSprite);
if (pirateList[i].health <= 0)
{ pirateList[i].Active = false; }
if (pirateList[i].Active == false)
{
pirateList[i].Body.Dispose();
pirateList[i].Geom.Dispose();
pirateList.RemoveAt(i);
}
}

//for (int i = 0; i < pirateList.Count(); i++)
//{
// if ((playerSprite.Position.X + 70 > pirateList[i].Position.X) && (playerSprite.Position.X - 70 < pirateList[i].Position.X))
// pirateList[i].Body.LinearVelocity.X = 0.0f;

// if (playerSprite.Position.X > pirateList[i].Position.X)
// pirateList[i].Body.LinearVelocity.X = pirateList[i].moveSpeed;

// else if (playerSprite.Position.X < pirateList[i].Position.X)
// pirateList[i].Body.LinearVelocity.X = -pirateList[i].moveSpeed;
//}
}

public void UpdateProjectiles(GameTime gameTime, Viewport viewport)
{
for (int i = projectiles.Count - 1; i >= 0; i--)
{
projectiles[i].Update(gameTime, viewport);
projectiles[i].Position += projectiles[i].movespeed;
if (projectiles[i].Position.X + projectiles[i].CurrentTexture.Width / 2 > viewport.Width + playerSprite.Position.X)// || Health <= 0)
{
projectiles[i].Active = false;
}
if (projectiles[i].Active == false)
{
projectiles[i].Body.Dispose();
projectiles[i].Geom.Dispose();
projectiles.RemoveAt(i);
}
}
}

private void ScrollCamera(Viewport viewport)
{
if (playerSprite.isinbossfight == false)
{
const float ViewMargin = 0.50f;
float marginWidth = viewport.Width * ViewMargin;
float marginLeft = cameraPosition + marginWidth;
float marginRight = cameraPosition + viewport.Width - marginWidth;

// Calculate the scrolling borders for the Y-Axis
const float TopMargin = 0.5f;
const float BottomMargin = 0.5f;
float marginTop = cameraPositionYAxis + viewport.Height * TopMargin;
float marginBottom = cameraPositionYAxis + viewport.Height - viewport.Height * BottomMargin;

// Calculate how far to scroll when the player is near the edges of the screen
float cameraMovement = 0.0f;
if (playerSprite.Position.X < marginLeft)
cameraMovement = playerSprite.Position.X - marginLeft;
else if (playerSprite.Position.X > marginRight)
cameraMovement = playerSprite.Position.X - marginRight;

// Calculate how far to vertically scroll when the player is near the top or bottom of the screen
float cameraMovementY = 0.0f;
if (playerSprite.Position.Y < marginTop)
cameraMovementY = playerSprite.Position.Y - marginTop;
else if (playerSprite.Position.Y > marginBottom)
cameraMovementY = playerSprite.Position.Y - marginBottom;

// Update the camera position but prevent scrolling off the ends of the level
float maxCameraPosition = ((map.levelHeight * map.textureSize) * (map.levelWidth * map.textureSize)) - viewport.Width;
float maxCameraPositionYOffset = ((map.levelHeight * map.textureSize) * (map.levelWidth * map.textureSize)) - viewport.Height;
cameraPositionYAxis = MathHelper.Clamp(cameraPositionYAxis + cameraMovementY, 0.0f, maxCameraPositionYOffset);
cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
}
}

private void DrawText()
{
if (bossList.Count() == 1)
{
spriteBatch.DrawString(font, "Boss Health: " + bossList[0].health, new Vector2(cameraPosition, cameraPositionYAxis + 650), Color.Black);
}
spriteBatch.DrawString(font, "Health: " + playerSprite.health, new Vector2(cameraPosition + 370, cameraPositionYAxis + 650), Color.Black);
spriteBatch.DrawString(font, "Level: " + map.currentlevel, new Vector2(cameraPosition + 1000, cameraPositionYAxis + 650), Color.Black);
}

public override void Draw(GameTime gameTime)
{
ScreenManager.GraphicsDevice.Clear(Color.White);
spriteBatch.Begin();
Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, -cameraPositionYAxis, 0.0f);
spriteBatch.End();
spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, cameraTransform);
//spriteBatch.Draw(backgroundTexture, viewportRect, Color.White);
map.Draw(spriteBatch);
spriteBatch.Draw(cursorTex, new Vector2(cursorPos.X - cursorTex.Width / 2, cursorPos.Y - cursorTex.Height / 2), Color.White);
if (playerSprite.isinbossfight == true)
{
for (int i = 0; i < bossList.Count(); i++)
{
spriteBatch.Draw(bossList[i].CurrentTexture, bossList[i].Position, null, Color.White, 0, bossList[i].Origin, 1, SpriteEffects.None, 1);
}
}
if (playerSprite.isidle == true)
{
spriteBatch.Draw(playerSprite.CurrentTexture, playerSprite.Position, null, Color.White, playerSprite.Rotation, playerSprite.Origin, 1, SpriteEffects.None, 1);
}
else if (playerSprite.isjumping == true)
{
spriteBatch.Draw(playerSprite.CurrentTexture, playerSprite.Position, null, Color.White, playerSprite.Rotation, playerSprite.Origin, 1, SpriteEffects.None, 1);
}
else
{
spriteBatch.Draw(playerSprite.CurrentTexture, playerSprite.Position, sourceRect, Color.White, playerSprite.Rotation, playerSprite.Origin, 1, SpriteEffects.None, 1);
}
spriteBatch.Draw(arm.currentTexture, arm.position, null, Color.White, arm.Rotation - MathHelper.PiOver2, arm.Origin, 1, SpriteEffects.None, 1);
for (int i = 0; i < pirateList.Count(); i++)
{
spriteBatch.Draw(pirateList[i].CurrentTexture, pirateList[i].Position, null, Color.White, 0, pirateList[i].Origin, 1, SpriteEffects.None, 1);
}
for (int i = 0; i < projectiles.Count(); i++)
{
spriteBatch.Draw(projectiles[i].CurrentTexture, projectiles[i].Position, null, Color.White, 0, projectiles[i].Origin, 1, SpriteEffects.None, 1);
}
DrawText();
DrawHud();
spriteBatch.End();
}

private void DrawHud()
{
spriteBatch.Draw(health.currentTexture, health.position, Color.White);
spriteBatch.Draw(gunwheellarge.currentTexture, gunwheellarge.position, Color.White);
spriteBatch.Draw(gunwheelsmall.currentTexture, gunwheelsmall.position, Color.White);
spriteBatch.Draw(gunwheelsmall2.currentTexture, gunwheelsmall2.position, Color.White);
spriteBatch.Draw(browser.currentTexture, browser.position, Color.White);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 120, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 45, progressbar.Width, 22), Color.Gray);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 120, (int)cameraPositionYAxis + 31, (int)(progressbar.Width * ((double)progress / ((map.levelWidth * map.textureSize)))), 22), new Rectangle(0, 45, progressbar.Width, 22), Color.LimeGreen);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 120, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 0, progressbar.Width, 22), Color.White);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 325, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 45, progressbar.Width, 22), Color.Gray);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 325, (int)cameraPositionYAxis + 31, (int)(progressbar.Width * ((double)progress / ((map.levelWidth * map.textureSize)))), 22), new Rectangle(0, 45, progressbar.Width, 22), Color.LimeGreen);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 325, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 0, progressbar.Width, 22), Color.White);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 530, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 45, progressbar.Width, 22), Color.Gray);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 530, (int)cameraPositionYAxis + 31, (int)(progressbar.Width * ((double)progress / ((map.levelWidth * map.textureSize)))), 22), new Rectangle(0, 45, progressbar.Width, 22), Color.LimeGreen);
spriteBatch.Draw(progressbar, new Rectangle((int)cameraPosition + 530, (int)cameraPositionYAxis + 31, progressbar.Width, 22), new Rectangle(0, 0, progressbar.Width, 22), Color.White);

}

}
}