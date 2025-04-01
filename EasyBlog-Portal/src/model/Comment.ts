import { AuthorResponse } from "./Authentication";

export interface CommentResponse {
  commentId: string;
  postId: string;
  author: AuthorResponse;
  content: string;
  parentId?: string;
  dateCreate: string;
  replies: CommentResponse[];
}
