import {useMutation} from "@tanstack/react-query";
import {anonymousInstance} from "../../axiosInstance.ts";
import {APIRoutes} from "../../../config/APIRoutes.ts";
import {httpErrorHandler} from "../../httpErrorHandler.ts";
import {AxiosError} from "axios";
import {useAuthContext} from "../../../context/AuthContext.tsx";
import {useNavigate} from "react-router";

type Response = {
  token: string;
  expires: Date | string;
  refreshToken: string;
  refreshTokenExpires: Date | string;
}

type LoginRequestBody = {
  email: string;
  password: string;
}

type RegisterRequestBody = LoginRequestBody & {
  name: string;
  lastname: string;
}

export const useAuthLogin = () => {
  const { setIsAuthenticated } = useAuthContext();
  const navigate = useNavigate();

  return useMutation<Response, AxiosError, LoginRequestBody>({
    mutationFn: async (data) => {
      const response = await anonymousInstance
        .post<Response>(APIRoutes.auth.login, data);
      return response.data;
    },
    onSuccess: (response) => {
      localStorage.setItem("token", response.token);
      setIsAuthenticated(true);
      navigate("/");
    },
    onError: (error) => httpErrorHandler({
      err: error,
      errorKeys: ["email", "password"],
      setIsAuthenticated,
    }),
  });
};

export const useAuthRegister = () => {
  const { setIsAuthenticated } = useAuthContext();
  const navigate = useNavigate();

  return useMutation<Response, AxiosError, RegisterRequestBody>({
    mutationFn: async (data) => {
      const response = await anonymousInstance
        .post<Response>(APIRoutes.auth.register, data);
      return response.data;
    },
    onSuccess: (response) => {
      localStorage.setItem("token", response.token);
      setIsAuthenticated(true);
      navigate("/");
    },
    onError: (error) => httpErrorHandler({
      err: error,
      errorKeys: ["email", "password", "name", "lastname"],
      setIsAuthenticated,
    }),
  })
}
