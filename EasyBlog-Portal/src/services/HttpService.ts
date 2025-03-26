import axios from "axios";
import { getCredentials } from "../hooks/useLogin";

const DoCallAPIWithToken = async <T>(
  url: string,
  method: string,
  requestBody?: T | FormData
) => {
  const token = getCredentials();

  const headers: Record<string, string> = {
    Authorization: `Bearer ${token}`,
  };
  if (!(requestBody instanceof FormData)) {
    headers["Content-Type"] = "application/json";
  }
  return axios({
    method: method,
    url: url,
    data: requestBody,
    headers: headers,
  });
};

const DoCallAPIWithOutToken = async <T>(
  url: string,
  method: string,
  requestBody?: T | FormData
) => {
  return axios({
    method: method,
    url: url,
    data: requestBody,
  });
};

export { DoCallAPIWithToken, DoCallAPIWithOutToken };