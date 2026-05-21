const BASE_URL = "http://localhost:5052/api";

const authHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

// Отримати список чатів
export async function fetchChats() {
  const res = await fetch(`${BASE_URL}/Chat`, { headers: authHeaders() });
  if (!res.ok) throw new Error("Failed to fetch chats");
  return res.json(); // ChatDto[]
}

// Отримати історію повідомлень
export async function fetchMessages(chatId: string, limit = 50) {
  const res = await fetch(`${BASE_URL}/Message/${chatId}?limit=${limit}`, {
    headers: authHeaders(),
  });
  if (!res.ok) throw new Error("Failed to fetch messages");
  return res.json(); // MessageDto[]
}

// Відправити повідомлення
export async function sendMessage(chatId: string, content: string) {
  const res = await fetch(`${BASE_URL}/Chat/send`, {
    method: "POST",
    headers: authHeaders(),
    body: JSON.stringify({ chatId, content }),
  });
  if (!res.ok) throw new Error("Failed to send message");
  return res.json();
}

export async function joinChat(chatId: string) {
  const res = await fetch(`${BASE_URL}/Chat/${chatId}/join`, {
    method: "POST",
    headers: authHeaders(),
  });
  if (!res.ok) throw new Error("Failed to join chat");
}

export async function leaveChat(chatId: string) {
  const res = await fetch(`${BASE_URL}/Chat/${chatId}/leave`, {
    method: "POST",
    headers: authHeaders(),
  });
  if (!res.ok) throw new Error("Failed to leave chat");
}

export async function createChat(name: string) {
  const res = await fetch(`${BASE_URL}/Chat`, {
    method: "POST",
    headers: authHeaders(),
    body: JSON.stringify(name),
  });
  if (!res.ok) throw new Error("Failed to create chat");
  return res.json(); // ChatDto
}