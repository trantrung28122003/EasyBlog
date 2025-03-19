package com.hutech.easylearning.service;


import com.hutech.easylearning.dto.reponse.BlogResponse;
import com.hutech.easylearning.dto.reponse.NotificationResponse;
import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.BlogCreationRequest;
import com.hutech.easylearning.dto.request.BlogLikeRequest;
import com.hutech.easylearning.dto.request.UserCreationRequest;
import com.hutech.easylearning.entity.Blog;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.enums.NotificationType;
import com.hutech.easylearning.exception.AppException;
import com.hutech.easylearning.exception.ErrorCode;
import com.hutech.easylearning.repository.BlogLikeRepository;
import com.hutech.easylearning.repository.BlogRepository;
import com.hutech.easylearning.repository.UserRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;

@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
public class BlogService {

    final UploaderService uploaderService;
    final UserService userService;
    private final BlogRepository blogRepository;
    private final UserRepository userRepository;
    private final SimpMessagingTemplate simpMessagingTemplate;
    private final BlogLikeRepository blogLikeRepository;
    private final NotificationService notificationService;

    public BlogResponse createBlog(BlogCreationRequest request, MultipartFile file) {
        var currentUser = userService.getMyInfo();
        String blogImageUrl = null;
        if(file != null)
        {
            blogImageUrl = uploaderService.uploadFile(file);
        }

        Blog blog = Blog.builder()
                .content(request.getContent())
                .imageUrl(blogImageUrl)
                .userId(currentUser.getId())
                .dateChange(LocalDateTime.now())
                .dateCreate(LocalDateTime.now())
                .changedBy(currentUser.getId())
                .isDeleted(false)
                .build();

        blogRepository.save(blog);
        int likeCount = blogLikeRepository.countByBlogId(blog.getId());
        BlogResponse blogResponse = BlogResponse.builder()
                .id(blog.getId())
                .likeCount(likeCount)
                .content(blog.getContent())
                .userFullName(currentUser.getFullName())
                .userAvatarUrl(currentUser.getImageUrl())
                .imageUrl(blogImageUrl)
                .dateCreate(blog.getDateCreate())
                .build();
        simpMessagingTemplate.convertAndSend("/topic/blogs", blogResponse);
        notificationService.addNotificationByPostBlog(blog.getId());
        return blogResponse;
    }

    public List<BlogResponse> getAllBlog() {
        var blogs = blogRepository.findAll();
        var currentUser = userService.getMyInfo();
        List<BlogResponse> blogResponses = new ArrayList<>();
        for(Blog blog : blogs)
        {
            int likeCount = blogLikeRepository.countByBlogId(blog.getId());
            int replyCount = 0;
            int commentCount = blog.getComments().size();
            for (var comment : blog.getComments()) {
                replyCount += comment.getReplies().size();
            }
            commentCount += replyCount;
            var userByBlogId = userRepository.findById(blog.getUserId()).orElse(null);
            boolean islike = blogLikeRepository.existsByBlogIdAndUserId(blog.getId(), currentUser.getId());
            BlogResponse blogResponse = BlogResponse.builder()
                    .id(blog.getId())
                    .likeCount(likeCount)
                    .commentCount(commentCount)
                    .content(blog.getContent())
                    .userFullName(userByBlogId.getFullName())
                    .userAvatarUrl(userByBlogId.getImageUrl())
                    .imageUrl(blog.getImageUrl())
                    .dateCreate(blog.getDateCreate())
                    .statusLikeByUser(islike)
                    .build();
            blogResponses.add(blogResponse);
        }
        blogResponses.sort((blog1, blog2) -> blog2.getDateCreate().compareTo(blog1.getDateCreate()));
        return blogResponses;
    }

}
