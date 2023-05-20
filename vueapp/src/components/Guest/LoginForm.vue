<script setup lang="ts">
import axios from "axios";
import { ref } from "vue";
import { VForm } from "vuetify/lib/components/index";
import { prefix } from "../../config";
import { router, user } from "../../main";

const visible = ref(false);
const loading = ref<boolean>(false);
const form_login = ref<typeof VForm | null>(null);
const form_reset = ref<typeof VForm | null>(null);

const email = ref<string>("");
const pass = ref<string>("");
const email_reset = ref<string>("");
const remember_me = ref<boolean>(false);

const page = ref<number>(1);

const send_mail = () => {};

const submit = async (data: SubmitEvent) => {
  await data;
  const login = await form_login.value?.validate();
  const reset = await form_reset.value?.validate();
  loading.value = true;
  if (login && login.valid) {
    try {
      const res = await axios.post(`${prefix}/api/auth/login`, {
        email: email.value,
        password: pass.value,
      });
      localStorage.setItem("token", res.data.accessToken);
      if (remember_me.value)
        localStorage.setItem("expireDate", res.data.expireDate);
      else {
        let time = new Date(Date.now());
        time.setTime(time.getTime() + 60 * 60 * 1000);
        localStorage.setItem("expireDate", time.toString());
      }
      localStorage.setItem("user", res.data.user);
      localStorage.setItem("role", res.data.role);
      user.name = res.data.user;
      user.isLoggedIn = true;
      user.role = res.data.role;
      console.log(user);

      router.push("/");
    } catch (error) {
      alert("Błędny e-mail lub hasło");
      console.log(error);
    }

    form_reset.value?.reset();
    form_login.value?.reset();
  }
  if (reset && reset.valid) {
    loading.value = true;
    alert(`E-mail: ${email_reset.value}`);
    page.value++;
    loading.value = false;
    form_reset.value?.reset();
    form_login.value?.reset();
  }
  loading.value = false;
};

const emailRules = [
  (value: string) => {
    if (value) return true;
    else return "Pole jest wymagane";
  },
  (value: string) =>
    /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(value) ||
    "Nieprawidłowy e-mail",
];

const passwordRules = [
  (value: string) => {
    if (value) return true;
    else return "Pole jest wymagane";
  },
  (value: string) => {
    if (value.length >= 8) return true;
    else return "Hasło musi zawierać przynajmniej 8 znaków";
  },
];
</script>

<template>
  <v-row justify="center" class="mx-2">
    <v-col xs="12" sm="6" md="3" align-self="center">
      <v-card elevation="5" class="rounded-lg" :loading="loading">
        <template v-slot:loader="{ isActive }">
          <v-progress-linear
            :active="isActive"
            color="deep-purple"
            height="4"
            indeterminate
          ></v-progress-linear>
        </template>
        <v-window v-model="page" direction="vertical" reverse>
          <v-window-item :value="1">
            <v-card-item>
              <v-container class="d-flex justify-center align-center">
                <v-card
                  height="64"
                  width="64"
                  color="#36BFF1"
                  class="d-flex justify-center align-center"
                >
                  <v-icon
                    icon="mdi-hospital-building"
                    size="48"
                    color="white"
                  ></v-icon>
                </v-card>
              </v-container>
              <v-card-title>Zaloguj się</v-card-title>
              <v-card-subtitle>Podaj dane logowania</v-card-subtitle>
            </v-card-item>
            <v-spacer></v-spacer>
            <v-card-text>
              <v-form ref="form_login" @submit.prevent="submit">
                <v-text-field
                  v-model="email"
                  type="email"
                  label="E-mail"
                  variant="solo"
                  :rules="emailRules"
                  class="py-1"
                  color="blue-darken-2"
                  required
                >
                </v-text-field>
                <v-text-field
                  v-model="pass"
                  label="Hasło"
                  variant="solo"
                  :type="visible ? 'text' : 'password'"
                  :append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
                  @click:append-inner="() => (visible = !visible)"
                  :rules="passwordRules"
                  class="py-1"
                  color="blue-darken-2"
                  required
                >
                </v-text-field>
                <v-checkbox
                  v-model="remember_me"
                  label="Zapamiętaj mnie"
                ></v-checkbox>
                <v-row justify="center">
                  <v-btn type="submit" color="blue-darken-2" class="mt-2"
                    >Zaloguj się</v-btn
                  >
                </v-row>
                <v-row justify="center">
                  <v-btn
                    variant="text"
                    size="small"
                    class="mt-2"
                    @click="page++"
                    >Zapomniałem hasła</v-btn
                  >
                </v-row>
              </v-form>
            </v-card-text>
          </v-window-item>
          <v-window-item :value="2">
            <v-card-item>
              <v-container class="d-flex justify-center align-center">
                <v-card
                  height="64"
                  width="64"
                  color="#36BFF1"
                  class="d-flex justify-center align-center"
                >
                  <v-icon
                    icon="mdi-hospital-building"
                    size="48"
                    color="white"
                  ></v-icon>
                </v-card>
              </v-container>
              <v-card-title>Przypomnij hasło</v-card-title>
              <v-card-subtitle>Podaj e-mail</v-card-subtitle>
            </v-card-item>
            <v-spacer></v-spacer>
            <v-card-text>
              <v-form ref="form_reset" @submit.prevent="submit">
                <v-text-field
                  v-model="email_reset"
                  type="email"
                  label="E-mail"
                  variant="solo"
                  :rules="emailRules"
                  class="py-1"
                  color="blue-darken-2"
                  required
                >
                </v-text-field>
                <v-row>
                  <v-col>
                    <v-btn
                      variant="outlined"
                      color="blue-darken-2"
                      class="mt-2"
                      @click="page--"
                      >Wstecz</v-btn
                    >
                  </v-col>
                  <v-col>
                    <v-btn type="submit" color="blue-darken-2" class="mt-2"
                      >Potwierdź</v-btn
                    >
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-window-item>
          <v-window-item :value="3">
            <v-card-item>
              <v-container class="d-flex justify-center align-center">
                <v-card
                  height="64"
                  width="64"
                  color="#36BFF1"
                  class="d-flex justify-center align-center"
                >
                  <v-icon
                    icon="mdi-hospital-building"
                    size="48"
                    color="white"
                  ></v-icon>
                </v-card>
              </v-container>
              <v-card-title>Przypomnij hasło</v-card-title>
              <v-card-subtitle>Na podany e-mail przesłano link</v-card-subtitle>
            </v-card-item>
            <v-spacer></v-spacer>
            <v-card-text>
              <v-form ref="form_reset" @submit.prevent="submit">
                <v-row>
                  <v-col>
                    <v-btn
                      variant="outlined"
                      color="blue-darken-2"
                      class="mt-2"
                      @click="page--"
                      >Wstecz</v-btn
                    >
                  </v-col>
                  <v-col>
                    <v-btn type="submit" color="blue-darken-2" class="mt-2"
                      >Zakończ</v-btn
                    >
                    <v-row justify="center">
                      <v-btn
                        variant="text"
                        size="small"
                        class="mt-4"
                        @click="send_mail"
                        >Wyślij e-mail ponownie</v-btn
                      >
                    </v-row>
                  </v-col>
                </v-row>
              </v-form>
            </v-card-text>
          </v-window-item>
        </v-window>
      </v-card>
    </v-col>
  </v-row>
</template>

<style></style>