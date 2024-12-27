package com.hutech.easylearning.service;

import com.hutech.easylearning.dto.reponse.CommentResponse;
import com.hutech.easylearning.dto.reponse.ReplyResponse;
import com.hutech.easylearning.dto.request.CommentRequest;
import com.hutech.easylearning.dto.request.ReplyRequest;
import com.hutech.easylearning.entity.Comment;
import com.hutech.easylearning.entity.Reply;
import com.hutech.easylearning.repository.CommentRepository;
import com.hutech.easylearning.repository.ReplyRepository;
import com.hutech.easylearning.repository.UserRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;


@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
public class CommentService {

    final CommentRepository commentRepository;
    final ReplyRepository replyRepository;
    final UserRepository userRepository;

    public List<CommentResponse> getCommentsByTrainingPartId(String trainingPartId) {
        List<CommentResponse> commentResponseList = new ArrayList<>();

        List<Comment> comments = commentRepository.findAllByBlogId(trainingPartId);
        for(Comment comment : comments) {
            var userOfComment = userRepository.findById(comment.getUserId()).orElseThrow();
            List<ReplyResponse> replyResponseList = new ArrayList<>();
            for(Reply reply : comment.getReplies()) {
                var userOfReply = userRepository.findById(reply.getUserId()).orElseThrow();
                ReplyResponse replyResponse = new ReplyResponse().builder()
                        .id(reply.getId())
                        .contentReply(reply.getContent())
                        .commentId(comment.getId())
                        .userId(reply.getUserId())
                        .userFullName(userOfReply.getFullName())
                        .userImageUrl(userOfReply.getImageUrl())
                        .dateCreate(reply.getDateCreate())
                        .build();
                replyResponseList.add(replyResponse);
            }

            CommentResponse commentResponse = new CommentResponse().builder()
                    .id(comment.getId())
                    .contentComment(comment.getContent())
                    .userId(comment.getUserId())
                    .userFullName(userOfComment.getFullName())
                    .userImageUrl(userOfComment.getImageUrl())
                    .dateCreate(comment.getDateCreate())
                    .replies(replyResponseList)
                    .build();


            commentResponseList.add(commentResponse);
        }
        return commentResponseList;
    }

    public CommentResponse addComment(CommentRequest request) {
        var userById = userRepository.findById(request.getUserId());
        Comment comment = new Comment().builder()
                .content(request.getCommentContent())
                .blogId(request.getTrainingPartId())
                .userId(request.getUserId())
                .dateCreate(LocalDateTime.now())
                .dateChange(LocalDateTime.now())
                .changedBy(request.getUserId())
                .build();
       commentRepository.save(comment);
       CommentResponse commentResponse = new CommentResponse().builder()
               .id(comment.getId())
               .contentComment(comment.getContent())
               .userId(comment.getUserId())
               .userFullName(userById.get().getFullName())
               .userImageUrl(userById.get().getImageUrl())
               .dateCreate(comment.getDateCreate())
               .replies(new ArrayList<>())
               .build();

        return commentResponse;
    }



    public ReplyResponse addReplyToComment(ReplyRequest request) {
        var userById = userRepository.findById(request.getCurrentUserId());
        Reply reply = new Reply().builder()
                .content(request.getReplyContent())
                .userId(request.getCurrentUserId())
                .dateCreate(LocalDateTime.now())
                .dateChange(LocalDateTime.now())
                .changedBy(request.getReplyContent())
                .commentId(request.getCommentId())
                .build();
        replyRepository.save(reply);


        ReplyResponse replyResponse = new ReplyResponse().builder()
                .id(reply.getId())
                .commentId(reply.getCommentId())
                .contentReply(reply.getContent())
                .userId(reply.getUserId())
                .userFullName(userById.get().getFullName())
                .userImageUrl(userById.get().getImageUrl())
                .dateCreate(reply.getDateCreate())
                .build();
        return replyResponse;
    }
}

