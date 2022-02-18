const path = require("path");
const { config } = require("process");

config = {
  name: "server",
  entry: "./src/index.js",
  target: "node",
  mode: "development",
  output: {
    path: path.resolve(process.cwd(), "dist"),
    filename: "server.js",
  },

  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: [
          {
            loader: "babel-loader",
            options: {
              presets: ["@babel/preset-react", "@babel/preset-env"],
              plugins: [
                "@babel/plugin-transform-runtime",
                "@babel/plugin-proposal-class-properties",
                "@babel/plugin-proposal-export-default-from",
              ],
            },
          },
          "source-map-loader",
        ],
      },
    ],
  },
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    process.env.NODE_ENV = "production";
    config.mode = "production";
  } else {
    process.env.NODE_ENV = "development";
  }

  return config;
};
