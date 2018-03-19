import moxios from 'moxios'
import axios from 'axios'

describe('Mocking axios requests', function () {
  describe('test', function () {
    beforeEach(function () {
      moxios.install()
    })

    afterEach(function () {
      moxios.uninstall()
    })

    it('Registration-Post', function (done) {
      moxios.withMock(function () {
        let onFulfilled = sinon.spy()
        axios.post('/Users/Registration').then(onFulfilled)

        moxios.wait(function () {
          let request = moxios.requests.mostRecent()
          request.respondWith({
            status: 200,
            response: {
              username: 'NotBilly'
            }
          }).then(function () {
            console.log('Registration - Done!')
            done()
          })
        })
      })
    })
  })
})
