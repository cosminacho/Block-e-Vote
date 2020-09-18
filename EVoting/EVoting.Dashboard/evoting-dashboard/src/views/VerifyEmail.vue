<template>
  <div>
    <img alt="Vue logo" src="../assets/logo.png" />
    <h2>{{ message }}</h2>
  </div>
</template>

<script>
// @ is an alias to /src
import axios from "axios";
export default {
  name: "VerifyEmail",
  data() {
    return {
      message: "Account verified succesfully!"
    };
  },
  created() {
    axios.defaults.baseURL = "https://localhost:44355/api/auth";
    let token = this.$route.query.token;
    let username = this.$route.query.username;
    if (token) {
      axios
        .post("/confirm-email", {
          Token: token,
          UserName: username
        })
        .then(res => {
          console.log(res.data);
          if (!res.data.success || !res.data.result) {
            this.message = "Couldn't verify email!";
          }
        })
        .catch(err => {
          console.log(err);
          this.message = "Couldn't verify email!";
        });
    } else {
      this.message = "Couldn't verify email!";
    }
  },
  components: {}
};
</script>
