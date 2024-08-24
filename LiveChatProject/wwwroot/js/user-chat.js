document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    const userName = document.getElementById('user-name').value;

    // Function to display a message on the chat board
    function displayMessage(senderName, message) {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');
        messageElement.textContent = `${senderName}: ${message}`;
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom
    }

    connection.on("ReceiveMessage", (senderName, message, isFromAgent) => {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');
        if (isFromAgent) {
            messageElement.textContent = `Agent: ${message}`;
        } else {
            messageElement.textContent = `${userName}: ${message}`;
        }
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom
    });

    connection.on("ReceiveChatHistory", (messages) => {
        const chatBoard = document.getElementById("chat-board");
        chatBoard.innerHTML = ''; // Clear the chat board before loading history
        messages.forEach(({ userId, agentId, message, isFromAgent }) => {
            const senderName = isFromAgent ? "Agent" : userName;
            displayMessage(senderName, message);
        });
    });

    // Start the connection
    connection.start().then(() => {
        console.log("Connected to SignalR hub");

        // Load chat history after connection starts
        const userId = document.getElementById('user-id').value;
        connection.invoke("LoadChatHistory", userId)
            .catch(err => console.error("Failed to load chat history:", err.toString()));
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
