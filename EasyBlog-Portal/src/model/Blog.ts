export interface BlogResponse {
    id: string;
    content: string;
    imageUrl: string;
    dateCreate: string; 
    likeCount: number;
    statusLikeByUser: boolean;
    commentCount: number;
    userFullName: string;
    userAvatarUrl: string;
}