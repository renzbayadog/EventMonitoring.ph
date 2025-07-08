# SignalR Chat Implementation for EventMonitoring.ph

This document describes the real-time chat functionality implemented using SignalR in the EventMonitoring.ph project.

## Features

### 🚀 Real-time Messaging
- Instant message delivery across all connected clients
- No page refresh required
- Automatic reconnection on connection loss

### 💬 Chat Rooms
- Multiple chat rooms support (general, events, support)
- Create custom chat rooms
- Room-specific user lists
- Join/leave room functionality

### 👥 User Presence
- Real-time user online status
- User join/leave notifications
- Live user count per room
- Connection status indicators

### 📱 Responsive Design
- Works on desktop and mobile devices
- Floating chat widget for quick access
- Full-screen chat interface
- Modern UI with gradient themes

## Components

### 1. CommunicationHub (Hubs/CommunicationHub.cs)
The main SignalR hub that handles all chat operations:
- `SendMessage()` - Broadcasts messages to room members
- `JoinChatRoom()` - Adds user to a chat room
- `LeaveChatRoom()` - Removes user from a chat room
- `GetUserList()` - Returns current users in a room
- Connection management with `OnConnectedAsync()` and `OnDisconnectedAsync()`

### 2. ChatService (Services/ChatService.cs)
Client-side service for managing SignalR connections:
- Automatic connection management
- Event handlers for real-time updates
- Room management operations
- Connection state monitoring

### 3. ChatPage (Components/Pages/Chat/ChatPage.razor)
Full-screen chat interface with:
- Sidebar showing rooms and online users
- Main chat area with message history
- Input area with send functionality
- Real-time message updates

### 4. ChatWidget (Components/Pages/Chat/ChatWidget.razor)
Floating chat widget that appears on all pages:
- Expandable/collapsible interface
- Unread message counter
- Connection status indicator
- Quick access to chat functionality

### 5. ChatMessage (Models/ChatMessage.cs)
Data model for chat messages:
- Message content
- Sender information
- Timestamp
- Room information
- Message ownership flag

## Setup and Configuration

### 1. SignalR Configuration (Program.cs)
```csharp
// Add SignalR services
builder.Services.AddSignalR();

// Map the hub
app.MapHub<CommunicationHub>("/communicationhub");
```

### 2. Service Registration
```csharp
// Register chat service
builder.Services.AddScoped<IChatService, ChatService>();
```

### 3. JavaScript Integration
The chat functionality includes JavaScript for enhanced UX:
- Auto-scroll to bottom on new messages
- Enter key handling for sending messages
- Connection status management

## Usage

### Accessing the Chat

1. **Full Chat Interface**: Navigate to `/chat` for the complete chat experience
2. **Chat Widget**: Available on all pages via the floating chat icon
3. **Demo Page**: Visit `/chat-demo` to see both implementations

### Using the Chat

1. **Sending Messages**:
   - Type your message in the input field
   - Press Enter or click the send button
   - Messages appear instantly for all users in the room

2. **Joining Rooms**:
   - Click on a room name in the sidebar
   - Messages are room-specific
   - User lists update automatically

3. **Creating Rooms**:
   - Enter a new room name
   - Click "Create" to add it to the available rooms

4. **Chat Widget**:
   - Click the chat icon to expand
   - Unread message count is shown
   - Connection status is indicated by the colored dot

## Technical Details

### SignalR Hub Methods

| Method | Description | Parameters |
|--------|-------------|------------|
| `SendMessage` | Broadcasts a message to room members | message, senderName, roomName |
| `JoinChatRoom` | Adds user to a chat room | roomName, userName |
| `LeaveChatRoom` | Removes user from a chat room | roomName, userName |
| `GetUserList` | Returns current users in a room | roomName |

### Client Events

| Event | Description | Data |
|-------|-------------|------|
| `ReceiveMessage` | New message received | ChatMessage object |
| `UserJoined` | User joined the room | userName, roomName |
| `UserLeft` | User left the room | userName, roomName |
| `UpdateUserList` | User list updated | List of usernames |

### Connection Management

- Automatic reconnection on connection loss
- Connection state monitoring
- Graceful disconnection handling
- User presence tracking

## Styling

The chat components use modern CSS with:
- Gradient backgrounds
- Smooth animations and transitions
- Responsive design
- Bootstrap integration
- Custom chat-specific styling

## Security Considerations

- User authentication integration
- Room-based message isolation
- Connection validation
- Input sanitization (implement as needed)

## Future Enhancements

Potential improvements for the chat system:
- Message persistence in database
- File/image sharing
- Typing indicators
- Message reactions
- Private messaging
- Message search functionality
- Chat history
- User avatars and profiles

## Troubleshooting

### Common Issues

1. **Connection Failed**:
   - Check if SignalR is properly configured in Program.cs
   - Verify the hub endpoint is accessible
   - Check browser console for errors

2. **Messages Not Appearing**:
   - Ensure you're in the correct chat room
   - Check connection status indicator
   - Verify SignalR hub is running

3. **Widget Not Showing**:
   - Check if ChatWidget is included in MainLayout.razor
   - Verify CSS is loading properly
   - Check for JavaScript errors

### Debug Information

Enable SignalR logging for debugging:
```csharp
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
```

## Dependencies

- Microsoft.AspNetCore.SignalR.Client (8.0.10)
- Microsoft.AspNetCore.SignalR (included in ASP.NET Core)
- Bootstrap (for styling)
- Font Awesome (for icons)

## Browser Support

- Chrome/Edge (recommended)
- Firefox
- Safari
- Mobile browsers (iOS Safari, Chrome Mobile)

The chat implementation provides a robust, real-time communication system that enhances user interaction in the EventMonitoring.ph application. 