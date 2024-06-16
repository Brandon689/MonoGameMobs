//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace MonoGameTopDown
//{
//    public class Game1 : Game
//    {
//        private GraphicsDeviceManager _graphics;
//        private SpriteBatch _spriteBatch;

//        // Player variables
//        private Texture2D _playerTexture;
//        private Vector2 _playerPosition;
//        private const float PlayerSpeed = 200f;

//        // Enemy variables
//        private Texture2D _enemyTexture;
//        private Vector2 _enemyPosition;
//        private const float EnemySpeed = 100f;
//        private List<Vector2> _enemyPath;
//        private int _pathIndex;

//        // Grid and walls
//        private int[,] _grid;
//        private List<Rectangle> _walls;
//        private const int GridSize = 20;
//        private const int WallSize = 1; // Wall size in grid units

//        // Line texture for drawing paths
//        private Texture2D _lineTexture;

//        // FSM for enemy states
//        private enum EnemyState
//        {
//            Patrolling,
//            Chasing,
//            Attacking
//        }

//        private EnemyState _currentEnemyState;
//        private Vector2 _patrolStart;
//        private Vector2 _patrolEnd;
//        private float _attackRange = 50f; // Example attack range

//        public Game1()
//        {
//            _graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";
//            IsMouseVisible = true;
//        }

//        protected override void Initialize()
//        {
//            base.Initialize();
//            _playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
//            _enemyPosition = new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4);

//            // Initialize grid and walls
//            int gridWidth = GraphicsDevice.Viewport.Width / GridSize;
//            int gridHeight = GraphicsDevice.Viewport.Height / GridSize;
//            _grid = new int[gridWidth, gridHeight];
//            _walls = new List<Rectangle>();

//            // Add some walls
//            for (int i = 5; i < 15; i++)
//            {
//                _walls.Add(new Rectangle(i, 10, WallSize, WallSize));
//                _walls.Add(new Rectangle(10, i, WallSize, WallSize));
//            }

//            // Add 4 more walls
//            _walls.Add(new Rectangle(3, 3, WallSize, 10));
//            _walls.Add(new Rectangle(7, 7, WallSize, WallSize));
//            _walls.Add(new Rectangle(12, 5, WallSize, WallSize));
//            _walls.Add(new Rectangle(15, 15, WallSize, 3));

//            foreach (var wall in _walls)
//            {
//                for (int x = wall.X; x < wall.X + wall.Width; x++)
//                {
//                    for (int y = wall.Y; y < wall.Y + wall.Height; y++)
//                    {
//                        _grid[x, y] = 1;
//                    }
//                }
//            }

//            _enemyPath = new List<Vector2>();
//            _pathIndex = 0;

//            // Initialize enemy state and patrol points
//            _currentEnemyState = EnemyState.Patrolling;
//            _patrolStart = new Vector2(100, 100); // Example patrol start point
//            _patrolEnd = new Vector2(200, 200); // Example patrol end point
//        }

//        protected override void LoadContent()
//        {
//            _spriteBatch = new SpriteBatch(GraphicsDevice);

//            // Create a 1x1 texture and set it to white for player
//            _playerTexture = new Texture2D(GraphicsDevice, 1, 1);
//            _playerTexture.SetData(new[] { Color.White });

//            // Create a 1x1 texture and set it to white for enemy
//            _enemyTexture = new Texture2D(GraphicsDevice, 1, 1);
//            _enemyTexture.SetData(new[] { Color.White });

//            // Create a 1x1 texture for drawing lines
//            _lineTexture = new Texture2D(GraphicsDevice, 1, 1);
//            _lineTexture.SetData(new[] { Color.White });
//        }

//        protected override void Update(GameTime gameTime)
//        {
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
//                Exit();

//            var keyboardState = Keyboard.GetState();
//            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

//            // Update player movement
//            Vector2 movement = Vector2.Zero;
//            if (keyboardState.IsKeyDown(Keys.W)) movement.Y -= 1;
//            if (keyboardState.IsKeyDown(Keys.S)) movement.Y += 1;
//            if (keyboardState.IsKeyDown(Keys.A)) movement.X -= 1;
//            if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;
//            if (movement != Vector2.Zero)
//                movement.Normalize();
//            _playerPosition += movement * PlayerSpeed * deltaTime;

//            // Update enemy behavior based on state
//            switch (_currentEnemyState)
//            {
//                case EnemyState.Patrolling:
//                    Patrol(deltaTime);
//                    break;
//                case EnemyState.Chasing:
//                    Chase(deltaTime);
//                    break;
//                case EnemyState.Attacking:
//                    Attack(deltaTime);
//                    break;
//            }

//            base.Update(gameTime);
//        }

//        private void Patrol(float deltaTime)
//        {
//            // Simple patrol logic: move between patrol points
//            Vector2 direction = _patrolEnd - _enemyPosition;
//            if (direction.Length() < 1f)
//            {
//                // Swap patrol points
//                var temp = _patrolStart;
//                _patrolStart = _patrolEnd;
//                _patrolEnd = temp;
//            }
//            else
//            {
//                direction.Normalize();
//                _enemyPosition += direction * EnemySpeed * deltaTime;
//            }

//            // Check if player is within sight range
//            if (Vector2.Distance(_enemyPosition, _playerPosition) < 200f) // Example sight range
//            {
//                _currentEnemyState = EnemyState.Chasing;
//            }
//        }

//        private void Chase(float deltaTime)
//        {
//            // Move towards the player
//            Vector2 direction = _playerPosition - _enemyPosition;
//            if (direction.Length() < _attackRange)
//            {
//                _currentEnemyState = EnemyState.Attacking;
//            }
//            else
//            {
//                direction.Normalize();
//                _enemyPosition += direction * EnemySpeed * deltaTime;
//            }
//        }

//        private void Attack(float deltaTime)
//        {
//            // Attack logic (e.g., reduce player health)
//            // For simplicity, just print a message
//            Console.WriteLine("Attacking the player!");

//            // If player moves out of attack range, switch back to chasing
//            if (Vector2.Distance(_enemyPosition, _playerPosition) > _attackRange)
//            {
//                _currentEnemyState = EnemyState.Chasing;
//            }
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.CornflowerBlue);

//            _spriteBatch.Begin();
//            // Draw the player as a circle
//            DrawCircle(_playerPosition, 20, Color.Red);
//            // Draw the enemy as a circle
//            DrawCircle(_enemyPosition, 20, Color.Green);
//            // Draw walls
//            foreach (var wall in _walls)
//                _spriteBatch.Draw(_playerTexture, new Rectangle(wall.X * GridSize, wall.Y * GridSize, wall.Width * GridSize, wall.Height * GridSize), Color.Black);
//            // Draw the enemy path
//            DrawPath(_enemyPath, Color.Yellow);
//            _spriteBatch.End();

//            base.Draw(gameTime);
//        }

//        private void DrawCircle(Vector2 center, int radius, Color color)
//        {
//            int diameter = radius * 2;
//            Texture2D circleTexture = new Texture2D(GraphicsDevice, diameter, diameter);
//            Color[] colorData = new Color[diameter * diameter];
//            float radiusSquared = radius * radius;
//            for (int x = 0; x < diameter; x++)
//            {
//                for (int y = 0; y < diameter; y++)
//                {
//                    int index = x * diameter + y;
//                    Vector2 pos = new Vector2(x - radius, y - radius);
//                    colorData[index] = pos.LengthSquared() <= radiusSquared ? color : Color.Transparent;
//                }
//            }
//            circleTexture.SetData(colorData);
//            _spriteBatch.Draw(circleTexture, center - new Vector2(radius, radius), Color.White);
//        }

//        private void DrawPath(List<Vector2> path, Color color)
//        {
//            if (path == null || path.Count < 2)
//                return;
//            for (int i = 0; i < path.Count - 1; i++)
//            {
//                Vector2 start = path[i] * GridSize + new Vector2(GridSize / 2, GridSize / 2);
//                Vector2 end = path[i + 1] * GridSize + new Vector2(GridSize / 2, GridSize / 2);
//                DrawLine(start, end, color);
//            }
//        }

//        private void DrawLine(Vector2 start, Vector2 end, Color color)
//        {
//            Vector2 edge = end - start;
//            float angle = (float)Math.Atan2(edge.Y, edge.X);
//            _spriteBatch.Draw(_lineTexture,
//                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1),
//                null,
//                color,
//                angle,
//                Vector2.Zero,
//                SpriteEffects.None,
//                0);
//        }

//        private List<Vector2> FindPath(Vector2 start, Vector2 goal)
//        {
//            var openList = new List<Node>();
//            var closedList = new HashSet<Node>();
//            var startNode = new Node(start, null, 0, GetHeuristic(start, goal));
//            openList.Add(startNode);
//            while (openList.Count > 0)
//            {
//                var currentNode = openList.OrderBy(node => node.F).First();
//                if (currentNode.Position == goal)
//                {
//                    return ReconstructPath(currentNode);
//                }
//                openList.Remove(currentNode);
//                closedList.Add(currentNode);
//                foreach (var neighbor in GetNeighbors(currentNode.Position))
//                {
//                    if (closedList.Any(node => node.Position == neighbor))
//                        continue;
//                    var tentativeG = currentNode.G + GetMovementCost(currentNode.Position, neighbor);
//                    var neighborNode = openList.FirstOrDefault(node => node.Position == neighbor);
//                    if (neighborNode == null)
//                    {
//                        neighborNode = new Node(neighbor, currentNode, tentativeG, GetHeuristic(neighbor, goal));
//                        openList.Add(neighborNode);
//                    }
//                    else if (tentativeG < neighborNode.G)
//                    {
//                        neighborNode.Parent = currentNode;
//                        neighborNode.G = tentativeG;
//                        neighborNode.F = neighborNode.G + neighborNode.H;
//                    }
//                }
//            }
//            return new List<Vector2>();
//        }

//        private List<Vector2> ReconstructPath(Node node)
//        {
//            var path = new List<Vector2>();
//            while (node != null)
//            {
//                path.Add(node.Position);
//                node = node.Parent;
//            }
//            path.Reverse();
//            return path;
//        }

//        private float GetHeuristic(Vector2 a, Vector2 b)
//        {
//            // Use Manhattan distance for grid-based movement
//            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
//        }

//        private List<Vector2> GetNeighbors(Vector2 position)
//        {
//            var neighbors = new List<Vector2>
//            {
//                new Vector2(position.X + 1, position.Y),
//                new Vector2(position.X - 1, position.Y),
//                new Vector2(position.X, position.Y + 1),
//                new Vector2(position.X, position.Y - 1),
//                new Vector2(position.X + 1, position.Y + 1),
//                new Vector2(position.X - 1, position.Y - 1),
//                new Vector2(position.X + 1, position.Y - 1),
//                new Vector2(position.X - 1, position.Y + 1)
//            };
//            return neighbors.Where(IsWalkable).ToList();
//        }

//        private bool IsWalkable(Vector2 position)
//        {
//            int x = (int)position.X;
//            int y = (int)position.Y;
//            return x >= 0 && y >= 0 && x < _grid.GetLength(0) && y < _grid.GetLength(1) && _grid[x, y] == 0;
//        }

//        private float GetMovementCost(Vector2 from, Vector2 to)
//        {
//            // Diagonal movement cost is √2 times the cost of horizontal/vertical movement
//            if (from.X != to.X && from.Y != to.Y)
//                return 1.414f; // Approximation of √2
//            return 1f;
//        }

//        private class Node
//        {
//            public Vector2 Position { get; }
//            public Node Parent { get; set; }
//            public float G { get; set; }
//            public float H { get; }
//            public float F { get; set; }

//            public Node(Vector2 position, Node parent, float g, float h)
//            {
//                Position = position;
//                Parent = parent;
//                G = g;
//                H = h;
//                F = g + h;
//            }
//        }
//    }
//}
