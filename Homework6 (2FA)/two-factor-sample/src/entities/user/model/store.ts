import { create } from 'zustand'
import { createJSONStorage, persist } from 'zustand/middleware'
import { type User } from './types'
import { api } from '@/shared'

interface UserState {
    user: User | null
    token: string | null
    isLoading: boolean

    setToken: (token: string) => void
    setUser: (user: User) => void
    logout: () => void
    fetchProfile: () => Promise<void>

    getIsAuth: () => boolean
}

export const useUserStore = create<UserState>()(
    persist(
        (set, get) => ({
            user: null,
            token: null,
            isLoading: false,

            getIsAuth: () => !!get().token,

            setToken: (token) => set((state) => ({ ...state, token })),

            setUser: (user) => set((state) => ({ ...state, user })),

            logout: () => {
                set({ user: null, token: null })
            },

            fetchProfile: async () => {
                const { token, user, isLoading } = get()

                if (!token || user || isLoading) return

                set({ isLoading: true })
                try {
                    const { data } = await api.get<User>('/profile')
                    set({ user: data })
                } catch {
                    set({ user: null, token: null })
                } finally {
                    set({ isLoading: false })
                }
            },
        }),
        {
            name: 'user-storage',
            storage: createJSONStorage(() => localStorage),
            partialize: (state) => ({ token: state.token }),
        },
    ),
)
