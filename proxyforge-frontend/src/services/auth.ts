import axios from 'axios';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await axios.post<LoginResponse>('/api/auth/login', request);
  // Store the JWT in localStorage for future requests
  localStorage.setItem('jwt', response.data.token);
  return response.data;
}

// Axios interceptor to attach JWT to all requests
autosetJwtInterceptor();

export function autosetJwtInterceptor() {
  axios.interceptors.request.use((config) => {
    const token = localStorage.getItem('jwt');
    if (token) {
      config.headers = config.headers || {};
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  });
}
