using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server_application
{
    public partial class Form1 : Form
    {
        // Creating a client struct to make things easier as we will have multiple 
        struct client
        {
            public Socket clientSocket;
            public string name;
            public char symbol;
            // Constructor for the client struct takes a socket object and a name 
            public client(Socket s, string n)
            {
                this.name = n;
                this.clientSocket = s;
                this.symbol = '\0';
            }
            // Method to assign a symbol to the player
            public void assignSymbol(char c)
            {
                this.symbol = c;
            }
        };
        // Create a new socket object for the server with stream socket type and TCP protocol
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        List<client> clients = new List<client>();

        Dictionary<string, int[]> stats = new Dictionary<string, int[]>();

        ManualResetEvent moveReceivedForThisTurn = new ManualResetEvent(false);

        // Necessary booleans for checks are defined
        bool terminating = false; // Flag to indicate if the program is terminating
        bool listening = false; // Flag to indicate if the program is listening for input
        bool isGamePlayed = false; // Flag to indicate if a game is currently being played
        bool gameOver = false; // Flag to indicate if the game is over

        int peopleInServer = 0; // Number of people currently connected to the server
        int xIdx, oIdx; // Index variables for X and O players
        char turnOf = '\0'; // Keep track of the current turn (X or O)
        char[,] currentGrid; // Game board represented as a 2D array


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false; // Disable illegal cross-thread calls check
            this.FormClosing += new FormClosingEventHandler(server_closing); // Subscribe to the FormClosing event with the server_closing method
            InitializeComponent(); // Initialize the form components
        }

        // Event handler method for the FormClosing event of the form
        private void server_closing(object sender, FormClosingEventArgs e)
        {
            listening = false; // Stop the server from listening for new connections
            terminating = true; // Server is in the process of terminating 
            BroadcastResult("[SERVER_FAILED]\n"); // Message to connected clients
            serverSocket.Close(); //Close the socket 
            Environment.Exit(0); //Exit the application 
        }

        private void listen_button_Click(object sender, EventArgs e)
        {
            // Get the text from the port_textbox and try parsing it as an integer.
            string port = port_textbox.Text;
            int serverPort;
            if (Int32.TryParse(port, out serverPort))
            {
                // Create an IPEndPoint with the server IP address and port number
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);

                // Assign a new a socket to the serverSocket like the previous one and bind it to an end point
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(6); //Listen client
                    listening = true;
                    listen_button.Enabled = false;
                    port_textbox.Enabled = false;

                    // Accepting thread to handle incoming client connection 
                    Thread acceptingThread = new Thread(accept);
                    acceptingThread.IsBackground = true;
                    acceptingThread.Start();

                    // Message to tell that server has started listening on the serverPort
                    server_richtextbox.AppendText("Started listening on port " + serverPort + "!\n");
                }
                catch
                {
                    // If there is an issue with listening to the specified port
                    server_richtextbox.AppendText("This port may be used by another application, try again!\n");
                }

            }
            else //Wrong port number warning is shown
                server_richtextbox.AppendText("Please check the port!\n");
        }

        // Takes the Socket object and the message to send to the socket as parameters
        private void sendMessage(Socket socket, string message)
        {
            // Checks if message is not empty and not too long
            if (message != "" && message.Length <= 1024)
            {
                // Convert string to buffer 
                Byte[] buffer = Encoding.Default.GetBytes(message);
                try
                {
                    // Try sending the byte array to the client socket
                    socket.Send(buffer);
                }
                catch
                {
                    // Error message is shown in case of exception while sending the message
                    server_richtextbox.AppendText("There is a problem! Check the connection...\n");
                    terminating = true;
                    // Reenable the listen_button and port_textbox and close the serverSocket 
                    port_textbox.Enabled = true;
                    listen_button.Enabled = true;
                    serverSocket.Close();
                }
            }
        }
        // Takes the Socket object as parameter 
        private string receiveMessage(Socket socket)
        {
            // Byte array to receive from socket
            Byte[] buffer = new Byte[1024];
            socket.Receive(buffer); // Receive the message as byte array
            string message = Encoding.Default.GetString(buffer); // Convert the message to string
            message = message.Substring(0, message.IndexOf("\0")); // Remove unused bytes 
            return message; // Return the string message 
        }

        //  Starts the game if it is not already started
        private void start_game()
        {
            if (!isGamePlayed)
            {
                start_button.Enabled = false;
                isGamePlayed = true;
                TicTacToe(); //Begin the game
                // If the game was played successfully 
                if (isGamePlayed)
                {
                    endGame(1); // Call the endGame method with 1
                }
            }
        }

        // Takes how it ended as a parameter
        private void endGame(int how)
        {
            // Broadcasts game over to the players
            BroadcastResult("[GAME OVER]\n");
            server_richtextbox.AppendText("[GAME OVER]\n");

            if (how == 1) // The game is played to the end
            {
                // Check the board to see who won
                string gameResult = CheckBoard(currentGrid);
                string winning_prompt = gameResult;

                if (gameResult.Contains("X")) // Player X won
                {
                    winning_prompt = clients[xIdx].name + " wins!\n";

                    // Update statistics for Player X
                    if (!stats.ContainsKey(clients[xIdx].name))
                    {
                        stats[clients[xIdx].name] = new int[4] { 1, 0, 0, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[xIdx].name];
                        ar[0] += 1;
                        ar[3] += 1;
                    }

                    // Update statistics for Player O
                    if (!stats.ContainsKey(clients[oIdx].name))
                    {
                        stats[clients[oIdx].name] = new int[4] { 0, 0, 1, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[oIdx].name];
                        ar[2] += 1;
                        ar[3] += 1;
                    }
                }
                else if (gameResult.Contains("O")) // Player O won
                {
                    winning_prompt = clients[oIdx].name + " wins!\n";

                    // Update statistics for Player X
                    if (!stats.ContainsKey(clients[xIdx].name))
                    {
                        stats[clients[xIdx].name] = new int[4] { 0, 0, 1, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[xIdx].name];
                        ar[2] += 1;
                        ar[3] += 1;
                    }

                    // Update statistics for Player O
                    if (!stats.ContainsKey(clients[oIdx].name))
                    {
                        stats[clients[oIdx].name] = new int[4] { 1, 0, 0, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[oIdx].name];
                        ar[0] += 1;
                        ar[3] += 1;
                    }
                }
                else // The game ended in a draw
                {
                    // Update statistics for Player X
                    if (!stats.ContainsKey(clients[xIdx].name))
                    {
                        stats[clients[xIdx].name] = new int[4] { 0, 1, 0, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[xIdx].name];
                        ar[1] += 1;
                        ar[3] += 1;
                    }

                    // Update statistics for Player O
                    if (!stats.ContainsKey(clients[oIdx].name))
                    {
                        stats[clients[oIdx].name] = new int[4] { 0, 1, 0, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[oIdx].name];
                        ar[1] += 1;
                        ar[3] += 1;
                    }
                }

                // Display the appropriate messages
                BroadcastResult(winning_prompt);
                server_richtextbox.AppendText(winning_prompt);

                // Reset the game state variables
                isGamePlayed = false;
                xIdx = oIdx = -1;

                string sts = ReturnStatistics();
                BroadcastResult(sts);
                server_richtextbox.AppendText(sts);

                Thread.Sleep(1000);

                if (peopleInServer >= 2) // Enough players so they can start again
                {
                    start_button.Enabled = true;
                    start_button.PerformClick();
                    start_button.Enabled = false;
                }
            }
            else // The game is not played to the end 
            {
                if (peopleInServer == 1) // Someone disconnected, so there is 1 player left
                {
                    // Remaining player wins and message is displayed
                    string res = clients[0].name + " is the winner, since their opponent disconnected!\n";
                    BroadcastResult(res);
                    server_richtextbox.AppendText(res);

                    // Reset the game state variables
                    isGamePlayed = false;
                    xIdx = oIdx = -1;

                    if (!stats.ContainsKey(clients[0].name))
                    {
                        stats[clients[0].name] = new int[4] { 1, 0, 0, 1 };
                    }
                    else
                    {
                        int[] ar = stats[clients[0].name];
                        ar[0] += 1;
                        ar[3] += 1;
                    }
                    // Check the number of people to enable or disable the start_button
                    if (peopleInServer < 2)
                    {
                        start_button.Enabled = false;
                    }
                    else
                    {
                        start_button.Enabled = true;
                    }
                }
                string sts = ReturnStatistics();
                BroadcastResult(sts);
                server_richtextbox.AppendText(sts);
            }

        }

        // Called in start_game method
        private void TicTacToe()
        {
            // isGamePlayed is set to true inside start_game method
            if (isGamePlayed)
            {
                turnOf = 'X'; // X starts the game
                currentGrid = PrepareEmptyGrid(); // Empty gameboard is prepared
                while (!gameOver) // While the game is not over 
                {
                    moveReceivedForThisTurn.Reset(); // Reset the event for the current turn
                    string board = ReturnBoardState(); //Store the current board state

                    // Send messages that contain the board depending on whose turn it is and change the turn afterwards
                    if (turnOf == 'X')
                    {
                        // It's Player X's turn
                        BroadcastResult("[" + clients[xIdx].name + "'s (X) Turn]\n" + board);
                        sendMessage(clients[oIdx].clientSocket, "Waiting for your turn...\n");
                        moveReceivedForThisTurn.WaitOne(); // Wait for the move to be received from Player O
                        turnOf = 'O'; // Change the turn to Player O
                    }
                    else
                    {
                        // It's Player O's turn
                        BroadcastResult("[" + clients[oIdx].name + "'s (O) Turn]\n" + board);
                        sendMessage(clients[xIdx].clientSocket, "Waiting for your turn...\n");
                        moveReceivedForThisTurn.WaitOne(); // Wait for the move to be received from Player X
                        turnOf = 'X'; // Change the turn to Player X
                    }
                    if (gameOver)
                    {
                        break;
                    }
                    // If checkBoard methods returns a non-empty string someone won the game
                    string res = CheckBoard(currentGrid);
                    if (res != "")
                    {
                        gameOver = true; // gameOver flag set to true
                    }
                }
            }
        }

        // Returns the current state of the tic-tac-toe board as a string
        private string ReturnBoardState()
        {
            String boardState = "";

            // Iterate through each cell in the 3x3 grid
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Add the cell to the boardState string
                    if (j != 2)
                        boardState += currentGrid[i, j] + " | ";
                    else
                        boardState += currentGrid[i, j];
                }

                // Move to the next row
                boardState += '\n';
            }

            // Return the resulting string
            return boardState;
        }

        // Creates a new empty 3x3 grid and fills it with the numbers 1 to 9
        private char[,] PrepareEmptyGrid()
        {
            char[,] grid = new char[3, 3];
            char currentChar = '1';

            // Fill the grid with numbers 1 to 9
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    grid[i, j] = currentChar;
                    currentChar++;
                }
            }

            // Return the resulting grid
            return grid;
        }

        private string ReturnStatistics()
        {
            string prompt = "\nCURRENT STATISTICS\n-----------------------------------------------\n\n";

            // Iterate through each player's statistics
            foreach (string key in stats.Keys)
            {
                prompt += key + ":\n" + "\tWins:" + stats[key][0].ToString() + "\tDraws:" + stats[key][1].ToString() + "\tLosses:" + stats[key][2].ToString() + "\tGames Played:" + stats[key][3].ToString() + "\n\n";
            }

            return prompt + "\n"; // Return the formatted statistics prompt
        }


        // Broadcasts a result string to all connected clients
        private void BroadcastResult(string result)
        {
            foreach (client c in clients)
            {
                try
                {
                    // Convert the result string to a byte array and send it to the client
                    Byte[] buffer = Encoding.Default.GetBytes(result);
                    c.clientSocket.Send(buffer);
                }
                catch
                {
                    // If the send fails, append an error message to the server_richtextbox control
                    server_richtextbox.AppendText("Could not send message to " + c.name + "\n");
                }
            }
        }

        // The method takes in a 2D array of characters representing a Tic-Tac-Toe board
        // and returns a string indicating the result of the game
        private string CheckBoard(char[,] board)
        {
            string result = "";

            // Check rows for a winning player
            for (int i = 0; i < 3; i++)
            {
                // Check if the first cell of the row is not empty and all cells in the row contain the same non-empty character
                if ((board[i, 0] == 'X' || board[i, 0] == 'O') && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                {
                    // Set the result to indicate the winning player and break out of the loop
                    result = board[i, 0] + " wins!";
                    break;
                }
            }

            // Check columns for a winning player
            for (int j = 0; j < 3; j++)
            {
                // Check if the first cell of the column is not empty and all cells in the column contain the same non-empty character
                if ((board[0, j] == 'X' || board[0, j] == 'O') && board[0, j] == board[1, j] && board[1, j] == board[2, j])
                {
                    // Set the result to indicate the winning player and break out of the loop
                    result = board[0, j] + " wins!";
                    break;
                }
            }

            // Check diagonals for a winning player
            if ((board[0, 0] == 'X' || board[0, 0] == 'O') && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                result = board[0, 0] + " wins!";
            else if ((board[0, 2] == 'X' || board[0, 2] == 'O') && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                result = board[0, 2] + " wins!";

            // If the result string does not contain "wins", it means there is no winner
            if (!result.Contains("wins"))
            {
                // Check if there are any empty cells left, if not it's a draw
                bool draw = true;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] >= '1' && board[i, j] <= '9')
                        {
                            draw = false;
                            break;
                        }
                    }
                }

                // If it's a draw, set the result string accordingly
                if (draw)
                    result = "It's a draw!\n";
            }

            // Return the result string
            return result;
        }

        private void accept()
        {
            while (listening) // keep running while the server is listening for incoming connections
            {
                try
                {
                    Socket newConnection = serverSocket.Accept(); // accept incoming connection
                    Byte[] buffer = new Byte[1024]; // create a buffer to receive data
                    newConnection.Receive(buffer); // receive data from the new connection
                    string rawData = Encoding.Default.GetString(buffer); // convert the data to a string
                    string client_name = rawData.Substring(0, rawData.IndexOf("\0")); // extract the client name from the received data
                    bool nameExists = false;
                    foreach (client c in clients) // iterate over connected clients to check if the name already exists
                    {
                        if (client_name == c.name)
                        {
                            nameExists = true;
                            break;
                        }
                    }
                    if (nameExists) // if the client name already exists, refuse the new connection
                    {
                        server_richtextbox.AppendText("A client with the name " + client_name + " is already connected, new connection refused!\n");
                        sendMessage(newConnection, "[NAME_IN_USE] Try again with a different name!\n");
                        newConnection.Close();
                    }
                    else if (peopleInServer >= 4) // if there are already two clients connected, refuse the new connection
                    {
                        server_richtextbox.AppendText("A client with the name " + client_name + " tried to connect, but server is full!\n");
                        sendMessage(newConnection, "[SERVER_FULL] Try again later, server is full!\n");
                        newConnection.Close();
                    }
                    else // if the new client can be connected, add the client to the list of connected clients and start a new thread to receive messages from the client
                    {
                        client newClient = new client(newConnection, client_name);
                        clients.Add(newClient);
                        peopleInServer++;
                        if (peopleInServer >= 2)
                        {
                            start_button.Enabled = true; // enable the start button if there are two or more clients connected
                        }
                        server_richtextbox.AppendText(client_name + " is connected to the server.\n");
                        sendMessage(newConnection, "[SUCCESS] You are connected!\n");
                        Thread receivingThread = new Thread(() => {
                            receive(ref newClient); // start a new thread to receive messages from the new client
                        });
                        receivingThread.IsBackground = true;
                        receivingThread.Start();
                    }
                }
                catch
                {
                    if (terminating) // if the server is terminating, stop listening for incoming connections
                        listening = false;
                    else if (listening)
                        server_richtextbox.AppendText("Socket stopped working!\n"); // log an error message if the socket stops working unexpectedly
                }
            }
        }

        private void receive(ref client newClient)
        {
            bool connected = true;

            while (connected && !terminating) // While the client is still connected and the server is not terminating
            {
                try
                {
                    string res = receiveMessage(newClient.clientSocket); // Receive a message from the client

                    if (res.Contains("[MOVE]")) // If the message contains "[MOVE]"
                    {
                        processMove(res, newClient); // Process the move sent by the client
                    }
                }
                catch
                {
                    if (!terminating) // If the server is not terminating
                    {
                        server_richtextbox.AppendText(newClient.name + " has disconnected from the server.\n"); // Print that the client has disconnected
                    }

                    newClient.clientSocket.Close(); // Close the client's socket
                    peopleInServer--; // Decrement the number of people in the server

                    if (isGamePlayed) // If the game is being played
                    {
                        // Remove the client from the list of connected clients
                        if (clients.IndexOf(newClient) < 2)
                        {

                            if (clients.Count == 2)
                            {
                                clients.Remove(newClient);
                                if (!stats.ContainsKey(newClient.name))
                                {
                                    stats[newClient.name] = new int[4] { 0, 0, 1, 1 };
                                }
                                else
                                {
                                    int[] ar = stats[newClient.name];
                                    ar[2] += 1;
                                    ar[3] += 1;
                                }
                                endGame(0); // End the game with 0 as the winner (no winner)
                                isGamePlayed = false; // Set the game as not being played
                                gameOver = true; // Set the game as over
                                moveReceivedForThisTurn.Set(); // Set the move received for this turn
                            }
                            else
                            {
                                // Determine the index of the new client in the clients list
                                int idx = clients.IndexOf(newClient);

                                // Get the name of the client being replaced
                                string clientName = clients[idx].name;

                                // Replace the client at index 2 with the new client and remove the old client
                                clients[idx] = clients[2];
                                clients.RemoveAt(2);

                                // Send a message to the new client, informing them that they will be playing in place of the replaced client
                                sendMessage(clients[idx].clientSocket, "[GAME_STARTING] You will be playing in place of " + clientName + "!\n");

                                // Check the turn and send the appropriate message to the new client
                                if (turnOf == 'X' && xIdx == idx)
                                {
                                    sendMessage(clients[idx].clientSocket, "[" + clients[idx].name + "'s (X) Turn]\n" + ReturnBoardState());
                                }
                                else if (turnOf == 'O' && oIdx == idx)
                                {
                                    sendMessage(clients[idx].clientSocket, "[" + clients[idx].name + "'s (O) Turn]\n" + ReturnBoardState());
                                }
                                else
                                {
                                    sendMessage(clients[idx].clientSocket, "Waiting for your turn...\n");
                                }
                            }
                        }
                        else
                        {
                            clients.Remove(newClient);
                        }
                    }
                    else
                    {
                        clients.Remove(newClient);
                    }

                    if (peopleInServer < 2) // If there are less than two people in the server
                    {
                        start_button.Enabled = false; // Disable the start button
                    }
                    connected = false; // The client is no longer connected
                }
            }
        }

        private void processMove(string res, client newClient)
        {
            string move = res.Split(' ')[1]; // Get the move from the message
            int place = Int32.Parse(move); // Parse the move as an integer
            int row = (place - 1) / 3; // Get the row index of the move
            int col = (place - 1) % 3; // Get the column index of the move
            currentGrid[row, col] = turnOf; // Set the current cell in the grid as the current player's mark
            BroadcastResult(newClient.name + " (" + turnOf + ") played: " + move + "\n"); // Broadcast the result of the move
            moveReceivedForThisTurn.Set(); // Set the move received for this turn
        }

        private void start_button_Click(object sender, EventArgs e)
        {
            if (clients.Count < 2)
            {
                // Check if there are at least two clients connected to the server
                server_richtextbox.AppendText("There are not enough clients in the server!\n");
            }
            else
            {
                start_button.Enabled = false;
                // Randomly choose which client will play X and which will play O
                Random random = new Random();
                xIdx = random.Next(0, 2);
                oIdx = 1 - xIdx;

                // Send game start message to both clients with their assigned symbol
                sendMessage(clients[xIdx].clientSocket, "[GAME_STARTING] You are X!\n");
                sendMessage(clients[oIdx].clientSocket, "[GAME_STARTING] You are O!\n");

                // Display game start message in the server log
                server_richtextbox.AppendText("[GAME_STARTING]\n");

                // Set game state to playing and start the game thread
                gameOver = false;
                Thread GameThread = new Thread(start_game);
                GameThread.IsBackground = true;
                GameThread.Start();
            }
        }
    }
}