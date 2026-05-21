import { useState, useEffect } from "react";
import { AuthScreen } from "./components/AuthScreen";
import { isTokenValid, getMyUserId } from "./utils/auth";
import { ChatSidebar } from "./components/ChatSidebar";
import { ChatView } from "./components/ChatView";
import { fetchChats, fetchMessages, sendMessage, createChat, joinChat, leaveChat } from "./components/api/chatApi";
import { useSignalR } from "./hooks/useSignalR";

interface Chat {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
}

interface Message {
  id: string;
  text: string;
  sender: "me" | "other";
  time: string;
}

function App() {
  const myUserId = getMyUserId();

  const [isAuthenticated, setIsAuthenticated] = useState(() =>
    isTokenValid(localStorage.getItem("token"))
  );
  const [chats, setChats] = useState<Chat[]>([]);
  const [selectedChatId, setSelectedChatId] = useState<string | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);

  // Модал створення
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newChatName, setNewChatName] = useState("");

  // Модал приєднання
  const [showJoinModal, setShowJoinModal] = useState(false);
  const [joinChatId, setJoinChatId] = useState("");

  useEffect(() => {
    if (!isAuthenticated) return;
    fetchChats().then((data: any[]) => {
      setChats(data.map((c) => ({
        id: c.id,
        name: c.name,
        lastMessage: c.lastMessage ?? "",
        time: c.lastMessageTime
          ? new Date(c.lastMessageTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })
          : "",
      })));
    });
  }, [isAuthenticated]);

  useEffect(() => {
    if (!selectedChatId) return;
    setMessages([]);
    fetchMessages(selectedChatId).then((data: any[]) => {
      setMessages(data.map((m) => ({
        id: m.id,
        text: m.content,
        sender: m.userId === myUserId ? "me" : "other",
        time: new Date(m.timestamp).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
      })));
    });
  }, [selectedChatId]);

  useSignalR(selectedChatId, (msg: any) => {
    setMessages((prev) => [...prev, {
      id: msg.id,
      text: msg.content,
      sender: msg.userId === myUserId ? "me" : "other",
      time: new Date(msg.timestamp).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
    }]);
  });

  const handleSendMessage = async (text: string) => {
    if (!selectedChatId) return;
    await sendMessage(selectedChatId, text);
  };

  const handleCreateChat = async () => {
  if (!newChatName.trim()) return;
  try {
    const created = await createChat(newChatName);
    setChats((prev) => [...prev, { id: created.id, name: created.name, lastMessage: "", time: "" }]);
    setNewChatName("");
    setShowCreateModal(false);
  } catch (err) {
    console.error("❌ createChat error:", err);
    alert(`Помилка: ${err}`);
  }
};

const handleJoinChat = async () => {
  if (!joinChatId.trim()) return;
  try {
    await joinChat(joinChatId.trim());
    const data = await fetchChats();
    setChats(data.map((c: any) => ({
      id: c.id, name: c.name, lastMessage: c.lastMessage ?? "", time: "",
    })));
    setJoinChatId("");
    setShowJoinModal(false);
  } catch (err) {
    console.error("❌ joinChat error:", err);
    alert(`Помилка: ${err}`);
  }
};

  const handleLeaveChat = async (chatId: string) => {
    await leaveChat(chatId);
    setChats((prev) => prev.filter((c) => c.id !== chatId));
    if (selectedChatId === chatId) setSelectedChatId(null);
  };

  const selectedChat = chats.find((c) => c.id === selectedChatId);

  return (
    <>
      {isAuthenticated ? (
        <div className="h-screen w-full flex">
          <ChatSidebar
            chats={chats}
            selectedChatId={selectedChatId}
            onSelectChat={setSelectedChatId}
            onCreateChat={() => setShowCreateModal(true)}
            onJoinChat={() => setShowJoinModal(true)}
            onLeaveChat={handleLeaveChat}
          />
          {selectedChat ? (
            <ChatView
              chatName={selectedChat.name}
              messages={messages}
              onSendMessage={handleSendMessage}
            />
          ) : (
            <div className="flex-1 flex items-center justify-center text-[#9b9b9b]">
              Оберіть чат
            </div>
          )}

          {/* Модал створення */}
          {showCreateModal && (
            <div className="fixed inset-0 bg-black/20 flex items-center justify-center z-50">
              <div className="bg-white rounded-3xl p-8 shadow-lg w-80 flex flex-col gap-4">
                <h2 className="text-lg text-[#2c2c2c]">Новий чат</h2>
                <input
                  autoFocus
                  type="text"
                  value={newChatName}
                  onChange={(e) => setNewChatName(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && handleCreateChat()}
                  placeholder="Назва чату"
                  className="px-5 py-3.5 bg-[#f5f5f3] rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
                />
                <div className="flex gap-3">
                  <button onClick={() => setShowCreateModal(false)} className="flex-1 py-3 rounded-full text-[#6b6b6b] hover:bg-[#f5f5f3] transition-colors">
                    Скасувати
                  </button>
                  <button onClick={handleCreateChat} className="flex-1 py-3 rounded-full bg-[#3d5a80] text-white hover:bg-[#2e4566] transition-colors">
                    Створити
                  </button>
                </div>
              </div>
            </div>
          )}

          {/* Модал приєднання */}
          {showJoinModal && (
            <div className="fixed inset-0 bg-black/20 flex items-center justify-center z-50">
              <div className="bg-white rounded-3xl p-8 shadow-lg w-80 flex flex-col gap-4">
                <h2 className="text-lg text-[#2c2c2c]">Приєднатися до чату</h2>
                <input
                  autoFocus
                  type="text"
                  value={joinChatId}
                  onChange={(e) => setJoinChatId(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && handleJoinChat()}
                  placeholder="Вставте ID чату"
                  className="px-5 py-3.5 bg-[#f5f5f3] rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20 font-mono text-sm"
                />
                <div className="flex gap-3">
                  <button onClick={() => setShowJoinModal(false)} className="flex-1 py-3 rounded-full text-[#6b6b6b] hover:bg-[#f5f5f3] transition-colors">
                    Скасувати
                  </button>
                  <button onClick={handleJoinChat} className="flex-1 py-3 rounded-full bg-[#3d5a80] text-white hover:bg-[#2e4566] transition-colors">
                    Приєднатися
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      ) : (
        <AuthScreen onLoginSuccess={() => setIsAuthenticated(true)} />
      )}
    </>
  );
}

export default App;