document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    // Function to display a message on the chat board
    function displayMessage(message) {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');
        messageElement.textContent = message; // Display only the message text
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom

    }

    connection.on("ReceiveMessage", (senderName, message, isFromAgent) => {
        displayMessage(message); // Only pass the message text to display
    });

    connection.on("ReceiveChatHistory", (messages) => {
        const chatBoard = document.getElementById("chat-board");
        chatBoard.innerHTML = ''; // Clear the chat board before loading history

        messages.forEach(({ message }) => {
            displayMessage(message); // Only pass the message text to display
        });

        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom after loading history
    });

    // Start the connection
    connection.start().then(() => {
        console.log("Connected to SignalR hub");

        //const userId = parseInt(document.getElementById('user-id').value);

        //if (isNaN(userId)) {
        //    console.error("Invalid userId");
        //    return;
        //}

        //connection.invoke("LoadChatHistory", userId,0)
        //    .then(() => console.log("Chat history loaded successfully"))
        //    .catch(err => console.error("Failed to load chat history:", err.toString()));
    }).catch(err => console.error("SignalR connection failed:", err.toString()));

    // Handle message sending from user to agent
    document.getElementById('message-form').addEventListener('submit', (event) => {
        event.preventDefault(); // Prevent form submission

        const message = document.getElementById('message-input').value;
        const userId = document.getElementById('user-id').value;

        if (message && userId) {
            connection.invoke("SendMessageToAgent", userId, message)
                .then(() => {
                    document.getElementById('message-input').value = ''; // Clear input after sending
                })
                .catch(err => console.error("Failed to send message:", err.toString()));
        } else {
            console.error("Message or user ID is missing");
        }
    });
});
