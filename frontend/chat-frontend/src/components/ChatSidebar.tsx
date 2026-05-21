import { MessageCircle, Plus, LogIn, LogOut, Copy, Check } from 'lucide-react';
import { useState } from 'react';

interface Chat {
  id: string;
  name: string;
  lastMessage: string;
  time: string;
}

interface ChatSidebarProps {
  chats: Chat[];
  selectedChatId: string | null;
  onSelectChat: (chatId: string) => void;
  onCreateChat: () => void;
  onJoinChat: () => void;
  onLeaveChat: (chatId: string) => void;
}

export function ChatSidebar({
  chats,
  selectedChatId,
  onSelectChat,
  onCreateChat,
  onJoinChat,
  onLeaveChat,
}: ChatSidebarProps) {
  const [copiedId, setCopiedId] = useState<string | null>(null);

  const handleCopyId = (e: React.MouseEvent, chatId: string) => {
    e.stopPropagation();
    navigator.clipboard.writeText(chatId);
    setCopiedId(chatId);
    setTimeout(() => setCopiedId(null), 2000);
  };

  return (
    <div className="w-80 bg-[#fafaf8] border-r border-[#e8e8e5] flex flex-col h-full">
      {/* Header */}
      <div className="p-6 border-b border-[#e8e8e5]">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <MessageCircle className="text-[#3d5a80]" size={28} strokeWidth={1.5} />
            <h1 className="text-xl text-[#2c2c2c]">Chats</h1>
          </div>
          <div className="flex gap-1">
            <button
              onClick={onJoinChat}
              className="p-2 rounded-full hover:bg-[#eeeeec] transition-colors text-[#3d5a80]"
              title="Приєднатися до чату"
            >
              <LogIn size={18} strokeWidth={1.5} />
            </button>
            <button
              onClick={onCreateChat}
              className="p-2 rounded-full hover:bg-[#eeeeec] transition-colors text-[#3d5a80]"
              title="Створити чат"
            >
              <Plus size={18} strokeWidth={1.5} />
            </button>
          </div>
        </div>
      </div>

      {/* Chat list */}
      <div className="flex-1 overflow-y-auto">
        {chats.map((chat) => (
          <div
            key={chat.id}
            className={`group border-b border-[#e8e8e5] transition-colors ${
              selectedChatId === chat.id ? 'bg-[#eeeeec]' : 'hover:bg-[#f5f5f3]'
            }`}
          >
            <button
              onClick={() => onSelectChat(chat.id)}
              className="w-full px-6 pt-4 pb-2 text-left"
            >
              <div className="flex items-start justify-between mb-1">
                <h3 className="font-medium text-[#2c2c2c]">{chat.name}</h3>
                <span className="text-xs text-[#9b9b9b]">{chat.time}</span>
              </div>
              <p className="text-sm text-[#6b6b6b] line-clamp-1">{chat.lastMessage}</p>
            </button>

            {/* ID + actions */}
            <div className="flex items-center justify-between px-6 pb-3 gap-2">
              <div className="flex items-center gap-1.5 min-w-0">
                <span className="text-xs text-[#b0b0a8] font-mono truncate">
                  {chat.id}
                </span>
                <button
                  onClick={(e) => handleCopyId(e, chat.id)}
                  className="shrink-0 text-[#b0b0a8] hover:text-[#3d5a80] transition-colors"
                  title="Скопіювати ID"
                >
                  {copiedId === chat.id
                    ? <Check size={12} />
                    : <Copy size={12} />
                  }
                </button>
              </div>

              <button
                onClick={() => onLeaveChat(chat.id)}
                className="shrink-0 opacity-0 group-hover:opacity-100 transition-opacity text-[#9b9b9b] hover:text-red-400 flex items-center gap-1"
                title="Вийти з чату"
              >
                <LogOut size={14} strokeWidth={1.5} />
                <span className="text-xs">Вийти</span>
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}