// Importing required namespaces
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

// Defining a namespace for the client application
namespace client_application
{
    // Creating a partial class for the form
    public partial class Form1 : Form
    {
        // Declaring some class level variables
        bool terminating = false;   // Indicates if the client is terminating
        bool connected = false;     // Indicates if the client is connected to the server
        bool gameStarted = false;   // Indicates if the game has started
        Socket clientSocket;        // A socket object to connect to the server
        char[,] currentBoard;       // A 2D char array to hold the current game board
        char playerSymbol;          // Keeping track of the symbol of the client

        // Constructor for the form
        public Form1()
        {
            // Disabling cross-threading checks (required to access UI components from other threads)
            Control.CheckForIllegalCrossThreadCalls = false;
            // Adding an event handler for the form closing event
            this.FormClosing += new FormClosingEventHandler(form_closing);
            // Initializing the components (UI)
            InitializeComponent();
        }

        // Event handler for the form closing event
        private void form_closing(object sender, FormClosingEventArgs e)
        {
            // Setting connected and terminating flags
            connected = false;
            terminating = true;
            // Exiting the application
            Environment.Exit(0);
        }

        // Method to send a message to the server
        private void sendMessage(Socket client, string msg)
        {
            // Checking if the message is not empty and its length is less than 64 characters
            if (msg != "" && msg.Length < 64)
            {
                // Converting the message to a byte array and sending it through the socket
                Byte[] buff = Encoding.Default.GetBytes(msg);
                client.Send(buff);
            }
        }

        // Method to receive a message from the server
        private string receiveMessage(Socket client)
        {
            // Creating a byte array to receive the message
            Byte[] buff = new Byte[1024];
            // Receiving the message from the server and storing it in the byte array
            client.Receive(buff);
            // Converting the byte array to a string and returning it
            string msg = Encoding.Default.GetString(buff);
            return msg.Substring(0, msg.IndexOf('\0'));
        }

        // Event handler for the connect button click event
        private void connect_button_Click(object sender, EventArgs e)
        {
            // Creating a socket object to connect to the server
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Getting the user input for IP, port and name
            string port = port_textbox.Text;
            string name = name_textbox.Text;
            string ip = ip_textbox.Text;
            IPAddress ipEndPoint;
            int portNum;
            // Checking if the name field is not empty
            if (name == "")
                server_richtextbox.AppendText("Your name cannot be empty!\n");
            // Checking if the IP address is valid
            else if (!IPAddress.TryParse(ip, out ipEndPoint))
                server_richtextbox.AppendText("Check the IP Address!\n");
            // Checking if the port number is valid and connecting to the server
            else if (Int32.TryParse(port, out portNum))
            {
                try
                {
                    clientSocket.Connect(ip, portNum);
                    sendMessage(clientSocket, name);
                    // Receiving the response from the server
                    string res = receiveMessage(clientSocket);
                    // Checking if the name is already in use
                    if (res.Contains("[NAME_IN_USE]"))
                    {
                        server_richtextbox.AppendText(res);
                        clientSocket.Close();
                    }
                    // If the server is full
                    else if (res.Contains("[SERVER_FULL]"))
                    {
                        server_richtextbox.AppendText(res);
                        clientSocket.Close();
                    }
                    // Otherwise, setting the connected flag and enabling/disabling UI components
                    else
                    {
                        connected = true;
                        connect_button.Enabled = false;
                        ip_textbox.Enabled = false;
                        name_textbox.Enabled = false;
                        port_textbox.Enabled = false;
                        disconnect_button.Enabled = true;
                        server_richtextbox.AppendText("Connected to the server!\n");
                        // Creating a new thread to receive messages from the server
                        Thread receivingThread = new Thread(Receive);
                        receivingThread.Start();
                    }
                }
                catch
                {
                    server_richtextbox.AppendText("There was an issue connecting to the server!\n");
                }
            }
            else
                server_richtextbox.AppendText("The input for port number needs to be an integer!\n");
        }

        // Method to receive messages from the server in a loop
        private void Receive()
        {
            while (connected)
            {
                try
                {
                    // Receiving a message from the server
                    string inMsg = receiveMessage(clientSocket);
                    // Checking if the game is starting
                    if (inMsg.Contains("[GAME_STARTING]"))
                    {
                        server_richtextbox.AppendText(inMsg);
                        playerSymbol = inMsg.ElementAt(24);
                        gameStarted = true;
                        // Starting the game
                        start_game();
                    }
                    // Checking if the server failed
                    else if (inMsg.Contains("[SERVER_FAILED]"))
                    {
                        throw new Exception();
                    }
                    else
                    {
                        server_richtextbox.AppendText(inMsg);
                    }
                }
                // Catching any exceptions and handling them appropriately
                catch
                {
                    if (!terminating)
                    {
                        server_richtextbox.Clear();
                        disconnect_button.Enabled = false;
                        send_button.Enabled = false;
                        ip_textbox.Enabled = true;
                        port_textbox.Enabled = true;
                        name_textbox.Enabled = true;
                        messages_textbox.Enabled = false;
                    }
                    clientSocket.Close();
                    connected = false;
                    connect_button.Enabled = true;
                }
            }
        }

        // Method to prepare an empty grid for the game
        private char[,] PrepareEmptyGrid()
        {
            // Creating a 3x3 char array and initializing it with numbers from 1 to 9
            char[,] grid = new char[3, 3];
            char currentChar = '1';
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    grid[i, j] = currentChar;
                    currentChar++;
                }
            return grid;
        }

        // Method to start the game
        private void start_game()
        {
            // Preparing an empty grid for the game
            currentBoard = PrepareEmptyGrid();
            // Receiving messages from the server while the game is running
            while (gameStarted)
            {
                // Receiving a message from the server
                string sv = receiveMessage(clientSocket);
                // Checking if it is the player's turn
                if (sv.Contains("[" + name_textbox.Text))
                {
                    server_richtextbox.AppendText(sv);
                    // Extracting the game board from the message and updating the current board
                    string sub = sv.Substring(sv.IndexOf("[" + name_textbox.Text));
                    string board = sub.Substring(sv.IndexOf('\n') + 1);
                    board = board.Substring(0, board.Length - 1);
                    updateBoard(board);
                    // Enabling the message input and send button
                    messages_textbox.Enabled = true;
                    send_button.Enabled = true;
                }
                else if (sv.Contains("[GAME_STARTING]"))
                {
                    server_richtextbox.AppendText(sv);
                }
                // Checking if the game is over
                else if (sv.Contains("[GAME OVER]"))
                {
                    server_richtextbox.AppendText(sv);
                    gameStarted = false;
                    send_button.Enabled = false;
                    messages_textbox.Enabled = false;
                    messages_textbox.Clear();
                }
                else
                {
                    server_richtextbox.AppendText(sv);
                }
            }
        }

        // Method to update the current game board based on the received board string
        private void updateBoard(string board)
        {
            // Removing the spaces from the board string and splitting it by new line characters
            string spacesRemoved = board.Replace(" ", "");
            string[] rows = spacesRemoved.Split('\n');

            int boardIdx = 1; // Index variable to keep track of the board position
            int i = 0; // Loop variable for rows

            // Looping through the rows and columns of the current board and updating it with the received values
            for (; i < 3; i++)
            {
                string[] cells = rows[i].Split('|'); // Split the row string into individual cells

                foreach (string cell in cells)
                {
                    int row = (boardIdx - 1) / 3; // Calculate the row index based on the board index
                    int col = (boardIdx - 1) % 3; // Calculate the column index based on the board index

                    currentBoard[row, col] = cell.ElementAt(0); // Update the current board with the value from the cell
                    boardIdx++;
                }
            }
        }


        // Event handler for the send button click event
        private void send_button_Click(object sender, EventArgs e)
        {
            // Extracting the move from the input field
            string move = messages_textbox.Text;
            int moveInt;
            // Checking if the move is a valid integer
            if (!Int32.TryParse(move, out moveInt))
            {
                server_richtextbox.AppendText("Enter an integer as a move!\n");
            }
            // Checking if the move is within the range of 1 to 9
            else if (Int32.Parse(move) < 1 || Int32.Parse(move) > 9)
            {
                server_richtextbox.AppendText("Please enter a valid cell!\n");
            }
            else
            {
                moveInt = Int32.Parse(move);
                int row = (moveInt - 1) / 3, col = (moveInt - 1) % 3;
                // Checking if the selected cell is not already played
                if (currentBoard[row, col] == 'X' || currentBoard[row, col] == 'O')
                {
                    server_richtextbox.AppendText("This cell is already played, try another cell!\n");
                }
                else
                {
                    // Creating a message to send to the server with the selected move
                    string message = "[MOVE] " + messages_textbox.Text;
                    // Checking if the message is not empty and its length is less than 64 characters
                    if (message != "" && message.Length <= 64)
                    {
                        // Converting the message to a byte array and sending it through the socket
                        Byte[] buffer = Encoding.Default.GetBytes(message);
                        try
                        {
                            clientSocket.Send(buffer);
                            // Disabling the message input and send button until the player's next turn
                            send_button.Enabled = false;
                            messages_textbox.Enabled = false;
                            messages_textbox.Clear();
                        }
                        catch
                        {
                            server_richtextbox.AppendText("There was a problem sending your message!\n");
                        }
                    }
                    else
                        server_richtextbox.AppendText("Your message needs to be at most 64 characters long and not empty!\n");
                }

            }
        }

        // Event handler for the disconnect button click event
        private void disconnect_button_Click(object sender, EventArgs e)
        {
            // Closing the socket
            clientSocket.Close();
            // Enabling/disabling UI components
            connected = false;
            connect_button.Enabled = true;
            disconnect_button.Enabled = false;
            ip_textbox.Enabled = true;
            port_textbox.Enabled = true;
            name_textbox.Enabled = true;
            messages_textbox.Enabled = false;
            send_button.Enabled = false;
            messages_textbox.Clear();
        }
    }
}
