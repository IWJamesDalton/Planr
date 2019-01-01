import Api from '~/app/Api'

export const state = () => ({
  loginUrl: '',
  token: '',
  logout: '',
  username: '',
  loggedIn: false
})
let api = {}
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
    if (!loggedIn) {
      sessionStorage.removeItem('authToken')
    } else {
      this.api = new Api(
        window.apiBaseUrl,
        window.functionKey,
        window.auth.token
      )
    }
    state.loggedIn = loggedIn
  },
  loadUsername(state) {
    this.api.getUsername().then(username => {
      state.username = username
    })
  }
}
