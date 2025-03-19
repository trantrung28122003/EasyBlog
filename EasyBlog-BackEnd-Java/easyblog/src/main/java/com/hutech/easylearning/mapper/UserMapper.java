package com.hutech.easylearning.mapper;


import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.UserCreationRequest;
import com.hutech.easylearning.dto.request.UserUpdateRequest;
import com.hutech.easylearning.entity.User;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.factory.Mappers;

@Mapper(componentModel = "spring")
public interface UserMapper {
    UserMapper INSTANCE = Mappers.getMapper(UserMapper.class);
    User toUser(UserCreationRequest request);
    UserResponse toUserResponse (User user);

    @Mapping(target = "roles", ignore = true)
    void updateUser(@MappingTarget User user , UserUpdateRequest request);
}
