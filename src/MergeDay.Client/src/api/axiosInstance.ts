import axios from 'axios'
import { env } from "../env";

export const anonymousInstance = axios.create({
  baseURL: env.API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const axiosInstance = axios.create({
  baseURL: env.API_URL,
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('token')}`
  }
})
