package com.hutech.easylearning.mapper;

import com.hutech.easylearning.dto.reponse.RoleResponse;
import com.hutech.easylearning.dto.request.RoleRequest;
import com.hutech.easylearning.dto.request.UserCreationRequest;
import com.hutech.easylearning.entity.Role;
import com.hutech.easylearning.entity.User;
import org.mapstruct.Mapper;

@Mapper(componentModel = "spring")
public interface RoleMapper {

    Role toRole(RoleRequest request);

    RoleResponse toRoleResponse(Role role);
}
