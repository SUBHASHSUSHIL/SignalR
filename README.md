# ASP.NET Core SignalR

A comprehensive ASP.NET Core (.NET 10) demo application exploring real-time communication patterns using SignalR — covering live user counts, group messaging, ordered notifications, and authenticated chat rooms.

---

## What is SignalR?

**ASP.NET Core SignalR** is an open-source library that simplifies adding real-time web functionality to applications. It enables the server to push content to connected clients instantly, rather than having the client repeatedly poll the server for updates.

Under the hood, SignalR uses a **Hub** — a high-level pipeline that allows the server and client to call methods on each other directly. Hubs abstract away the underlying transport so you write the same application code regardless of how the connection is physically made.

---

## Transport Types

SignalR automatically negotiates the best available transport between client and server. There are three transport mechanisms, tried in order of preference:

### 1. WebSockets
> **Preferred transport**

WebSockets establish a full-duplex, persistent TCP connection between client and server. Once the handshake completes (via an HTTP Upgrade request), both sides can send data frames at any time without re-establishing the connection.

| Characteristic | Detail |
|---|---|
| Protocol | `ws://` / `wss://` |
| Direction | Bidirectional (simultaneous send & receive) |
| Latency | Lowest — no HTTP overhead after handshake |
| Connection | Persistent, single long-lived TCP connection |
| Browser support | All modern browsers |

**Best for:** High-frequency updates, gaming, live collaboration, financial tickers.

---

### 2. Server-Sent Events (SSE)
> **Fallback #1**

SSE uses a standard HTTP response that is kept open indefinitely. The server streams `text/event-stream` data to the client over this connection. The client cannot send data back over the same channel — it uses separate HTTP requests for that.

| Characteristic | Detail |
|---|---|
| Protocol | Standard HTTP |
| Direction | Server → Client only (unidirectional) |
| Latency | Low — persistent HTTP response stream |
| Connection | One persistent HTTP connection |
| Browser support | All modern browsers (not IE) |

**Best for:** Live feeds, dashboards, and notification streams where server-to-client is the primary data flow.

---

### 3. Long Polling
> **Fallback #2 (last resort)**

Long polling simulates server push over plain HTTP. The client sends a request and the server holds it open until it has new data (or a timeout occurs), then responds. The client immediately issues a new request, creating the illusion of a persistent connection.

| Characteristic | Detail |
|---|---|
| Protocol | Standard HTTP |
| Direction | Bidirectional (via repeated request/response cycles) |
| Latency | Higher — each message requires a full HTTP round-trip |
| Connection | New HTTP request per message cycle |
| Browser support | Universal — works everywhere |

**Best for:** Environments where WebSockets and SSE are blocked by proxies or firewalls (e.g., corporate networks).

---

### Transport Negotiation Flow

```
Client connects
      │
      ▼
  WebSockets available?
      │ Yes ──► Use WebSockets ✓
      │ No
      ▼
  SSE available?
      │ Yes ──► Use Server-Sent Events ✓
      │ No
      ▼
  Use Long Polling ✓
```

SignalR handles this negotiation automatically. You can also pin a specific transport in the client configuration if needed.

---

## Why Use SignalR?

| Challenge | Without SignalR | With SignalR |
|---|---|---|
| Real-time updates | Client polls every N seconds, wasting bandwidth | Server pushes the moment data changes |
| Transport compatibility | You pick one transport and handle fallbacks manually | Automatic negotiation — works everywhere |
| Bi-directional RPC | Raw WebSocket messages need custom serialization | Call server/client methods by name with typed arguments |
| Connection lifecycle | Manual reconnection logic, heartbeat management | Built-in automatic reconnect, heartbeats, and scale-out |
| Scale-out | Need custom pub/sub infrastructure | Drop-in backplane support (Redis, Azure SignalR Service) |
| Groups & targeting | DIY connection registry | Built-in `Groups`, `Users`, and `Clients` abstractions |

In short, SignalR removes the plumbing of real-time communication so you focus on what your application actually does — not on how bytes travel between server and browser.

---

## Hubs in This Project

| Hub | Route | Purpose |
|---|---|---|
| `UserHub` | `/hubs/userCount` | Tracks and broadcasts live connected user count |
| `DeathlyHallowsHub` | `/hubs/deathyhallows` | Demonstrates a race/competition status board |
| `HouseGroupHub` | `/hubs/houseGroup` | Group messaging — clients join/leave named groups |
| `NotificationHub` | `/hubs/notification` | Broadcast and load persistent notifications |
| `BasicChatHub` | `/hubs/basicchat` | Simple unauthenticated chat (broadcast to all) |
| `ChatHub` | `/hubs/chat` | Authenticated chat with per-user connection tracking |
| `OrderHub` | `/hubs/order` | Real-time order status updates |

---

## Project Setup

### 1. Add the SignalR client-side library

Right-click the project solution → **Add** → **Client-side library**

- **Provider:** `unpkg`
- **Library:** `@microsoft/signalr@latest`
- **Target location:** `wwwroot/lib/microsoft/signalr/`

<img width="1260" height="631" alt="Add Client-Side Library dialog" src="https://github.com/user-attachments/assets/5a3b4ed8-ea94-48db-8f1e-2dc9d46267e0" />

### 2. Register SignalR in `Program.cs`

```csharp
builder.Services.AddSignalR();

// Map each hub to a route
app.MapHub<UserHub>("/hubs/userCount");
app.MapHub<ChatHub>("/hubs/chat");
// ... other hubs
```

### 3. Reference the client script

```html
<script src="~/lib/microsoft/signalr/dist/browser/signalr.min.js"></script>
```

### 4. Connect from JavaScript

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/userCount")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

await connection.start();
```

---

## Tech Stack

- **Framework:** ASP.NET Core (.NET 10)
- **Real-time:** ASP.NET Core SignalR
- **Auth:** ASP.NET Core Identity
- **Database:** Entity Framework Core + SQL Server
- **Client:** JavaScript SignalR client (`@microsoft/signalr`)
