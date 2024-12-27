export interface CommentReponse {
    id: string;
    userId: string;
    userFullName: string;
    userImageUrl: string;
    dateCreate: string;
    contentComment: string;
    replies: Reply[];
  }


export  interface Reply {
    id: string;
    userId: string;
    userFullName: string;
    userImageUrl: string;
    dateCreate: string;
    contentReply: string;
  }