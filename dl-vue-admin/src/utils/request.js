import axios from 'axios'
import { MessageBox, Message } from 'element-ui'
import store from '@/store'
import { getToken } from '@/utils/auth'
import router from '@/router'

// create an axios instance
const instance = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // true: send cookies when cross-domain requests
  // timeout: 25000, // request timeout,
  headers: {
    'Content-Type': 'application/json'
  }
})
// request interceptor
instance.interceptors.request.use(
  config => {
    // do something before request is sent
    var token = getToken()
    if (token) {
      config.headers['Authorization'] = token // 让每个请求携带自定义token 请根据实际情况自行修改
    }
    return config
  },
  error => {
    // do something with request error
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
instance.interceptors.response.use(
  response => {
    const res = response.data
    if (response.headers.token) {
      // 如果后台通过header返回token，说明token已经更新，则更新客户端本地token
      store.dispatch('user/updateToken', { token: response.headers.token })
    }
    // if the custom code is not 20000, it is judged as an error.
    if (res.code !== 20000) {
      debugger
      Message({
        message: res.msg || 'error',
        type: 'error',
        duration: 5 * 1000
      })

      // 50008: Illegal token; 50012: Other clients logged in; 50014: Token expired;
      if (res.code === 50008 || res.code === 50012 || res.code === 50014) {
        // to re-login
        MessageBox.confirm('You have been logged out, you can cancel to stay on this page, or log in again', 'Confirm logout', {
          confirmButtonText: 'Re-Login',
          cancelButtonText: 'Cancel',
          type: 'warning'
        }).then(() => {
          store.dispatch('user/resetToken').then(() => {
            location.reload()
          })
        })
      }
      return Promise.reject(res)
    } else {
      return res
    }
  },
  error => {
    debugger
    if (error.response.status === 401) {
      store.dispatch('user/logout').then(() => {
        router.replace({
          path: '/login',
          query: { redirect: router.currentRoute.path }
        })
      })
      return
    }
    const msg = error.response.data.message || error.response.data
    Message({
      message: msg || 'unknown error, please contact website maintainer',
      type: 'error',
      duration: 5 * 1000
    })
    return Promise.reject(error)
  }
)

export default instance
