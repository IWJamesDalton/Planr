import axios from 'axios'

class Api {
  constructor(baseUrl, functionKey, authToken) {
    this.baseUrl = baseUrl
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
    const dateString = date.toISOString()
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
      const fullName = userDetails.user_claims.find(function(i) {
        return i.typ === 'name'
      })
      return fullName ? fullName.val : null
    })
  }
}

export default Api
