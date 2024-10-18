//"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();



"use strict";

// Establish a connection to the SignalR hub
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established
document.getElementById("sendButton").disabled = true;

// Get the current user's name from the input
var currentUser = document.getElementById("userInput").value;

connection.on("ReceiveMessage", function (user, message) {
    var p = document.createElement("p");
    var div = document.createElement("div");  // Create a new div for each message
    var box = document.createElement("div");  // Create a new box for each message

    // Differentiate between messages from the current user and others
    if (user === currentUser) {
        div.classList.add("justify-content-end");
        box.style.backgroundColor = "rgba(57, 192, 237, .2)";
        p.style.textAlign = 'right';
    } else {
        div.classList.add("justify-content-start");
        p.style.textAlign = 'left';
    }

    p.textContent = message;
    box.classList.add("p-3", "ms-3", "bg-light"); // Add necessary classes to the box
    box.style.borderRadius = "15px";
    box.appendChild(p);

    div.classList.add("d-flex", "mb-2"); // Add necessary classes to the div
    div.appendChild(box);

    // Append new message to the same container as old messages
    var messagesList = document.getElementById("messagesList");
    messagesList.appendChild(div);

 
   
        // Get the input field
        var inputField = document.getElementById('messageInput');

        // Perform your send message operation here
        // Example: console.log(inputField.value);

        // Clear the input field
        inputField.value = '';
    
   

    // Scroll to the bottom of the messages list
    messagesList.scrollTop = messagesList.scrollHeight;
});

// Start the connection and enable the send button
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

// Handle send button click event
document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;

    // Invoke the SendMessage method on the server
    connection.invoke("SendMessage", receiver, user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
function scrollToBottom() {
    var messagesList = document.getElementById("messagesList");
    messagesList.scrollTop = messagesList.scrollHeight;
}

// Call this function when the page loads and when new messages arrive
window.addEventListener('load', scrollToBottom);
connection.on("ReceiveMessage", function (user, message) {
    // Existing message handling code...

    // Scroll to the bottom after appending the new message
    scrollToBottom();
});