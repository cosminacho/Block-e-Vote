<template>
  <div class="container">
    <div>
      <div>
        <h2>Current Camera</h2>
        <code v-if="device">{{ device.label }}</code>
        <div class="border">
          <vue-web-cam
            ref="webcam"
            :device-id="deviceId"
            width="100%"
            @started="onStarted"
            @stopped="onStopped"
            @error="onError"
            @cameras="onCameras"
            @camera-change="onCameraChange"
          />
        </div>

        <div>
          <div>
            <select v-model="camera">
              <option>-- Select Device --</option>
              <option
                v-for="device in devices"
                :key="device.deviceId"
                :value="device.deviceId"
                >{{ device.label }}</option
              >
            </select>
          </div>
          <div>
            <button type="button" class="btn btn-primary" @click="onCapture">
              Capture Photo
            </button>
            <button type="button" class="btn btn-danger" @click="onStop">
              Stop Camera
            </button>
            <button type="button" class="btn btn-success" @click="onStart">
              Start Camera
            </button>
          </div>
        </div>
      </div>
      <div>
        <h2>Captured Image</h2>
        <figure class="figure">
          <img :src="img" class="img-responsive" />
        </figure>
      </div>
      <div>
        <input type="text" v-model="username" />
      </div>
      <button class="btn btn-primary" @click="login">Login User</button>
    </div>
  </div>
</template>

<script>
import { WebCam } from "vue-web-cam";
import Swal from "sweetalert2";
import axios from "axios";

export default {
  name: "App",
  components: {
    "vue-web-cam": WebCam
  },
  created() {
    axios.defaults.baseURL = "https://localhost:44355/api/auth";
    axios.defaults.headers["Content-Type"] = "multipart/form-data";
  },
  data() {
    return {
      img: null,
      camera: null,
      deviceId: null,
      username: "",
      devices: []
    };
  },
  computed: {
    device: function() {
      return this.devices.find(n => n.deviceId === this.deviceId);
    }
  },
  watch: {
    camera: function(id) {
      this.deviceId = id;
    },
    devices: function() {
      // Once we have a list select the first one
      const [first, ...tail] = this.devices;
      console.log(tail);
      if (first) {
        this.camera = first.deviceId;
        this.deviceId = first.deviceId;
      }
    }
  },
  methods: {
    login() {
      fetch(this.img)
        .then(res => res.blob())
        .then(blob => {
          var file = new File([blob], "test.jpeg");

          var fd = new FormData();
          fd.append("image", file);
          fd.append("username", this.username);

          axios
            .post("/login-voter-face", fd)
            .then(res => {
              console.log(res);
              res = res.data;
              if (res.success && res.result) {
                Swal.fire("Success", "User authentificated", "success");
              } else {
                Swal.fire("Error", "There was an error on the server", "error");
              }
            })
            .catch(err => {
              Swal.fire("Error", "There was an error on the server", "error");

              console.log(err);
            });
        });
    },

    onCapture() {
      this.img = this.$refs.webcam.capture();
    },
    onStarted(stream) {
      console.log("On Started Event", stream);
    },
    onStopped(stream) {
      console.log("On Stopped Event", stream);
    },
    onStop() {
      this.$refs.webcam.stop();
    },
    onStart() {
      this.$refs.webcam.start();
    },
    onError(error) {
      console.log("On Error Event", error);
    },
    onCameras(cameras) {
      this.devices = cameras;
      console.log("On Cameras Event", cameras);
    },
    onCameraChange(deviceId) {
      this.deviceId = deviceId;
      this.camera = deviceId;
      console.log("On Camera Change Event", deviceId);
    }
  }
};
</script>
