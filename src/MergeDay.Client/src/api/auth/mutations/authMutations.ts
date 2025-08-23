import {useMutation} from "@tanstack/react-query";
import {anonymousInstance} from "../../axiosInstance.ts";
import {APIRoutes} from "../../../config/APIRoutes.ts";
import {httpErrorHandler} from "../../httpErrorHandler.ts";
import {AxiosError} from "axios";
import {useAuthContext} from "../../../context/AuthContext.tsx";

type ResponseBody = string;

type RequestBody = {
  email: string;
  password: string;
}

export const useAuthLogin = () => {
  const { setIsAuthenticated } = useAuthContext();

  return useMutation<ResponseBody, AxiosError, RequestBody>({
    mutationFn: async (data) => {
      const response = await anonymousInstance
        .post<ResponseBody>(APIRoutes.auth.login, data);
      return response.data;
    },
    onSuccess: (token) => {
      localStorage.setItem("token", token);
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

  return useMutation<ResponseBody, AxiosError, RequestBody>({
    mutationFn: async (data) => {
      const response = await anonymousInstance
        .post<ResponseBody>(APIRoutes.auth.register, data);
      return response.data;
    },
    onSuccess: (token) => {
      localStorage.setItem("token", token);
    },
    onError: (error) => httpErrorHandler({
      err: error,
      errorKeys: ["email", "password"],
      setIsAuthenticated,
    }),
  })
}
