import axios from 'axios'

class Api {
  constructor(baseUrl, blobBaseUrl, functionKey, authToken) {
    this.baseUrl = baseUrl
    this.blobBaseUrl = blobBaseUrl
    this.authToken = authToken
    this.functionKey = functionKey
  }

  getDay(date) {
    const config = {
      headers: {
        'x-functions-key': this.functionKey,
        'X-ZUMO-AUTH': this.authToken
      }
    }
    const dateString = `${date.getUTCFullYear()}${date.getUTCMonth() +
      1}${date.getUTCDate()}`
    return axios
      .get(`${this.baseUrl}/api/day/date?date=${dateString}`, config)
      .then(response => {
        if (response.status === 200) {
          return response.data
        } else {
          throw response.data
        }
      })
  }

  addDay(day) {
    const config = {
      headers: {
        'x-functions-key': this.functionKey,
        'X-ZUMO-AUTH': this.authToken
      }
    }
    return axios
      .post(`${this.baseUrl}/api/addday`, day, config)
      .then(response => {
        if (response.status === 200) {
          return response.data
        } else {
          throw response.data
        }
      })
  }

  getUsername() {
    const config = {
      headers: {
        'x-functions-key': this.functionKey,
        'X-ZUMO-AUTH': this.authToken
      }
    }
    return axios.get(`${this.baseUrl}/.auth/me`, config).then(response => {
      const userDetails = response.data[0]
      const userFirstName = userDetails.user_claims.find(function(i) {
        return (
          i.typ ===
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname'
        )
      })
      return userFirstName ? userFirstName.val : null
    })
  }
}

export default Api
