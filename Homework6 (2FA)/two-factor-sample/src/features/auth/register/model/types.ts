export interface RegisterRequest {
    userName: string
    phoneNumber: string
    password: string
}

export interface RegisterResponse {
    message: string
    userId?: string
}
