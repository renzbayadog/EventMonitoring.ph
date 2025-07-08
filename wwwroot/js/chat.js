// Chat JavaScript functionality
window.scrollToBottom = function (element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Auto-scroll to bottom when new messages arrive
window.autoScrollToBottom = function () {
    const messagesContainer = document.querySelector('.chat-messages');
    if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
};

// Handle enter key press in chat input
window.handleChatKeyPress = function (event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        const sendButton = event.target.parentElement.querySelector('button');
        if (sendButton && !sendButton.disabled) {
            sendButton.click();
        }
    }
};

// Initialize chat functionality when page loads
window.initializeChat = function () {
    // Auto-scroll to bottom on page load
    setTimeout(() => {
        window.autoScrollToBottom();
    }, 100);

    // Add event listeners for chat input
    const chatInput = document.querySelector('.chat-input input');
    if (chatInput) {
        chatInput.addEventListener('keypress', window.handleChatKeyPress);
    }
};

// Clean up chat functionality
window.cleanupChat = function () {
    const chatInput = document.querySelector('.chat-input input');
    if (chatInput) {
        chatInput.removeEventListener('keypress', window.handleChatKeyPress);
    }
}; 