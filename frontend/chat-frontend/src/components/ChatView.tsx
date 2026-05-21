import { useState } from 'react';
import { Send } from 'lucide-react';

interface Message {
  id: string;
  text: string;
  sender: 'me' | 'other';
  time: string;
}

interface ChatViewProps {
  chatName: string;
  messages: Message[];
  onSendMessage: (text: string) => void;
}

export function ChatView({ chatName, messages, onSendMessage }: ChatViewProps) {
  const [inputValue, setInputValue] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (inputValue.trim()) {
      onSendMessage(inputValue);
      setInputValue('');
    }
  };

  return (
    <div className="flex-1 flex flex-col h-screen bg-white">
      {/* Header */}
      <div className="px-8 py-6 border-b border-[#e8e8e5]">
        <h2 className="text-xl text-[#2c2c2c]">{chatName}</h2>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-8 py-6 space-y-4">
        {messages.map((message) => (
          <div
            key={message.id}
            className={`flex ${message.sender === 'me' ? 'justify-end' : 'justify-start'}`}
          >
            <div className="max-w-md">
              <div
                className={`px-5 py-3 rounded-3xl ${
                  message.sender === 'me'
                    ? 'bg-[#3d5a80] text-white'
                    : 'bg-[#f5f5f3] text-[#2c2c2c]'
                }`}
              >
                <p className="leading-relaxed">{message.text}</p>
              </div>
              <p className="text-xs text-[#9b9b9b] mt-1.5 px-2">
                {message.time}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Input */}
      <div className="px-8 py-6 border-t border-[#e8e8e5]">
        <form onSubmit={handleSubmit} className="flex gap-3">
          <input
            type="text"
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            placeholder="Type a message..."
            className="flex-1 px-5 py-3.5 bg-[#f5f5f3] border-none rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
          />
          <button
            type="submit"
            className="px-6 py-3.5 bg-[#3d5a80] text-white rounded-full hover:bg-[#2c4560] transition-colors flex items-center gap-2"
          >
            <Send size={18} />
          </button>
        </form>
      </div>
    </div>
  );
}
