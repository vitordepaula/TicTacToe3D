using System;

class TicTacToe3D {

    private int[,,] board;
    private int player; // 1: Player 1, 2: Player 2 (CPU)
    private int mask = (1 << 4) - 1;

    public TicTacToe3D(int size)
    {
        board = new int[size, size, size];
        player = 1; // Start with player 1
    }

    public bool PlacePiece(int x, int y, int z)
    {
        if (board[x, y, z] == 0)
        {
            board[x, y, z] = player;
            //Console.WriteLine("Player {0} placed a piece on {1}, {2}, {3}", player, x, y, z);
            player = (player == 1) ? 2 : 1; // switch sides
            return true;
        }
        //Console.WriteLine("Unable to place player ${0} piece on {1}, {2}, {3}", player, x, y, z);
        return false;
    }

    public bool PlacePiece(int move)
    {
        return PlacePiece(move >> 8, (move >> 4) & mask, move & mask);
    }

    public int CheckWinner()
    {
        // Check rows, columns, diagonals in all 3 dimensions (x, y, z)
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (IsWinner(i, 0, 0) || IsWinner(0, i, 0) || IsWinner(0, 0, i))
            {
                return board[i, 0, 0];
            }
        }

        // Check diagonals across all dimensions
        if (IsWinner(0, 0, 0) || IsWinner(board.GetLength(0) - 1, 0, 0) ||
            IsWinner(0, 0, board.GetLength(2) - 1) || IsWinner(0, board.GetLength(1) - 1, 0) ||
            IsWinner(board.GetLength(0) - 1, board.GetLength(1) - 1, 0) ||
            IsWinner(board.GetLength(0) - 1, 0, board.GetLength(2) - 1) ||
            IsWinner(0, board.GetLength(1) - 1, board.GetLength(2) - 1) ||
            IsWinner(board.GetLength(0) - 1, board.GetLength(1) - 1, board.GetLength(2) - 1))
        {
            return board[0, 0, 0];
        }

        // Check for tie
        if (IsBoardFull())
        {
            return 0; // Tie
        }

        return -1; // No winner yet
    }

    private bool IsWinner(int x, int y, int z)
    {
        int value = board[x, y, z];
        if (value == 0)
        {
            return false;
        }

        // Check all directions in 3 dimensions
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0)
                    {
                        continue; // Skip the current cell
                    }

                    int count = 0;
                    for (int i = 0; i < board.GetLength(0); i++)
                    {
                        int nx = x + i * dx;
                        int ny = y + i * dy;
                        int nz = z + i * dz;

                        if (nx >= 0 && nx < board.GetLength(0) &&
                            ny >= 0 && ny < board.GetLength(1) &&
                            nz >= 0 && nz < board.GetLength(2) &&
                            board[nx, ny, nz] == value)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count >= 4)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsBoardFull()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                for (int k = 0; k < board.GetLength(2); k++)
                {
                    if (board[i, j, k] == 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public int CPUMove()
    {
        // Prioritize winning moves (offensive)
        int move = OffensiveMove();
        if (move != -1)
        {
            return move;
        }

        // Block opponent's winning moves (defensive)
        move = DefensiveMove();
        if (move != -1)
        {
            return move;
        }

        // Choose a random empty cell (fallback)
        return RandomMove();
    }

    private int OffensiveMove()
    {
        // Check all empty cells for potential winning moves
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int z = 0; z < board.GetLength(2); z++)
                {
                    if (board[x, y, z] == 0)
                    {
                        // Simulate placing the CPU piece at this cell
                        board[x, y, z] = player;
                        if (CheckWinner() == player)
                        {
                            // Found a winning move for CPU, return it
                            board[x, y, z] = 0; // Revert the simulation
                            return (x << 8) | (y << 4) | z; // Encode the move as a single integer
                        }
                        board[x, y, z] = 0; // Revert the simulation
                    }
                }
            }
        }

        return -1; // No winning move found
    }

    private int DefensiveMove()
    {
        // Check all empty cells for potential opponent's winning moves
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                for (int z = 0; z < board.GetLength(2); z++)
                {
                    if (board[x, y, z] == 0)
                    {
                        // Simulate placing opponent's piece at this cell
                        int oponnent = (player == 1) ? 2 : 1;
                        board[x, y, z] = oponnent; // Opponent's piece
                        if (CheckWinner() == oponnent) // Check winner for opponent
                        {
                            // Found a potential losing move for CPU, block it
                            board[x, y, z] = 0; // Revert the simulation
                            return (x << 8) | (y << 4) | z; // Encode the move as a single integer
                        }
                        board[x, y, z] = 0; // Revert the simulation
                    }
                }
            }
        }

        return -1; // No opponent's winning move found
    }

    private Random random = new Random();

    private int RandomMove()
    {
        // Choose a random empty cell
        while (true)
        {
            int x = random.Next(board.GetLength(0));
            int y = random.Next(board.GetLength(1));
            int z = random.Next(board.GetLength(2));
            if (board[x, y, z] == 0)
            {
                return (x << 8) | (y << 4) | z; // Encode the move as a single integer
            }
        }
    }

    int[,,] GetBoard() {
        return board;
    }

    static void Main(string[] args) {
        // Test - put CPU to play with itself
        TicTacToe3D game = new TicTacToe3D(4);
        int winner;
        do {
            int move = game.CPUMove();
            if (!game.PlacePiece(move)) { // placing piece also switches player
                Console.WriteLine("SOMETHING WENT WRONG, COULD NOT PLACE PIECE");
                break;
            }
            winner = game.CheckWinner();
        } while (winner == -1);
        switch(winner) {
            case 0: Console.WriteLine("It's a tie!"); break;
            case 1:
            case 2: Console.WriteLine("Player {0} wins!", winner); break;
            default: Console.WriteLine("ERROR! SHOULD NOT HAPPEN, GAME STILL GOING."); break;
        }
        int[,,] board = game.GetBoard();
        Console.WriteLine("Board:");
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
               for (int k = 0; k < 4; k++) {
                   int piece = board[i,j,k];
                   Console.Write((piece == 1) ? 'O' : ((piece == 2) ? 'X' : ' '));
               }
               Console.Write('|');
            }
            Console.WriteLine();
        }
    }

}
