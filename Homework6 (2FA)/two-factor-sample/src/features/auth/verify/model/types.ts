export interface Verify2FARequest {
    userId: string
    code: string
    provider: 'Phone' | 'Authenticator'
}

export interface Verify2FAResponse {
    token: string
    user: {
        id: string
        userName: string
        phoneNumber: string
    }
}
