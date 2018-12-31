export const state = () => ({
  loginUrl: '',
  token: '',
  logout: '',
  username: null,
  loggedIn: false
})

export const mutations = {
  login(state) {
    window.location.href = state.loginUrl
  },
  logout(state) {
    state.loggedIn = false
  },
  setLoginUrl(state, loginUrl) {
    state.loginUrl = loginUrl
  },
  setToken(state, token) {
    state.token = token
  },
  setLoggedIn(state, loggedIn) {
    state.loggedIn = loggedIn
  },
  setUsername(state, username) {
    state.username = username
  }
}
