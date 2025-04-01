import { AuthorResponse } from "./Authentication";
import { CommentResponse } from "./Comment";

export interface PostResponse {
    id: string;
    title?: string;
    content?: string;
    author: AuthorResponse;
    dateCreated: string;
    imageUrls: string[];
    commentsResponse: CommentResponse[];
    likeCount: number;
    commentCount: number;
    isLiked: boolean;
    isSaved: boolean;
  }
  

  

