export interface Message {
    id: number;
    senderId: number;
    senderUsername: string;
    senderImageUrl: string;
    recipientId: number;
    recipientUserName: string;
    recipientImageUrl: string;
    content: string;
    dateRead?: Date;
    messageSent: Date;
}
