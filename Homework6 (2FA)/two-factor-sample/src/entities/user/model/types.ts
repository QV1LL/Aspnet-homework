export type TwoFactorMethod = 'Sms' | 'Totp'

export interface User {
    id: string
    userName: string
    phoneNumber: string
    enabledTwoFactorMethods: TwoFactorMethod[]
}
