package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Comment;
import com.hutech.easylearning.entity.Reply;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ReplyRepository extends JpaRepository<Reply, String> {

}

